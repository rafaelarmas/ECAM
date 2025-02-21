using ECAM.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECAM.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;

    private readonly Data.AccountDataContext _context;

    public AccountController(ILogger<AccountController> logger, Data.AccountDataContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpPost]
    [Route("meter-reading-uploads")]
    public async Task<ActionResult<MeterReadingUploadResults>> MeterReadingUpload(IFormFile file)
    {
        try
        {
            return await ReadMeterReadings(file.OpenReadStream());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Meter reading Upload failed.");
            return BadRequest(ex);
        }
    }

    [HttpGet]
    [Route("get-all-acounts")]
    public async Task<ActionResult<List<Account>>> GetAllAccounts()
    {
        try
        {
            return await _context.Accounts.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get All Accounts failed.");
            return BadRequest(ex);
        }
    }

    private async Task<MeterReadingUploadResults> ReadMeterReadings(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException("File has no data.");

        var results = new MeterReadingUploadResults();
        var streamReader = new StreamReader(stream);
        string? line;

        while ((line = streamReader.ReadLine()) != null)
        {
            var values = line.Split(',');

            // TODO: Add better validation.
            int accountId;
            bool isValidAccount = int.TryParse(values[0], out accountId);

            if (!isValidAccount)
            {
                // CSV file header should not count as a failed reading.
                if (values[0] == "AccountId")
                    continue;

                results.Failed++;
                continue;
            }

            var account = await _context.Accounts.SingleOrDefaultAsync(x => x.Id == accountId);

            if (account == null)
            {
                results.Failed++;
                continue;
            }

            var meterReading = new MeterReading();
            DateTime inputDateTime;
            var isValidDateTime = DateTime.TryParse(values[1], out inputDateTime);
            if (!isValidDateTime)
            {
                results.Failed++;
                continue;
            }

            int inputInt;
            var isValidInt = int.TryParse(values[2], out inputInt);
            if (!isValidInt || inputInt < 0)
            {
                results.Failed++;
                continue;
            }

            meterReading.MeterReadingDateTime = inputDateTime;
            meterReading.MeterReadValue = inputInt;

            // Check previous readings are older
            var previousMeterReading = await _context.MeterReadings
                .OrderByDescending(x => x.MeterReadingDateTime)
                .FirstOrDefaultAsync(r => account.MeterReadings.Contains(r.Id));

            if (previousMeterReading != null && (DateTime.Compare(previousMeterReading.MeterReadingDateTime, meterReading.MeterReadingDateTime) >= 0))
            {
                results.Failed++;
                continue;
            }

            _context.MeterReadings.Add(meterReading);
            await _context.SaveChangesAsync();
            
            account.MeterReadings.Add(meterReading.Id);
            await _context.SaveChangesAsync();

            results.Successful++;
        }
        return results;
    }
}

import React, { useState } from 'react';

const SingleFileUploader = () => {
    const [file, setFile] = useState<File | null>(null);

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files) {
            setFile(e.target.files[0]);
        }
    };

    const handleUpload = async () => {
        if (file) {
            console.log('Uploading file...');

            const formData = new FormData();
            formData.append('file', file);

            try {
                const result = await fetch('http://localhost:5051/meter-reading-uploads', {
                    method: 'POST',
                    mode: 'cors',
                    credentials: 'include',
                    body: formData
                });

                const data = await result.json();
                
                console.log(data);
                alert('Success: ' + data.successful + ', Failed: ' + data.failed);
            } catch (error) {
                console.error(error);
            }
        }
    };

    return (
        <>
            <div className="input-group">
                <input id="file" type="file" onChange={handleFileChange} />
            </div>
            {file && (
                <section>
                    File details:
                    <ul>
                        <li>Name: {file.name}</li>
                        <li>Type: {file.type}</li>
                        <li>Size: {file.size} bytes</li>
                    </ul>
                </section>
            )}

            {file && (
                <button
                    onClick={handleUpload}
                    className="submit"
                >Upload file</button>
            )}
        </>
    );
};

export default SingleFileUploader;
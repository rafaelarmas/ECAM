import './App.css'
import SingleFileUploader from './components/SingleFileUploader.tsx'
function App() {

  return (
    <>
        <h1>ECAM</h1>
        <SingleFileUploader />
        <p className="read-the-docs">Please select a CSV file to upload.</p>
    </>
  )
}

export default App

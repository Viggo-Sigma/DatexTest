import React, { useState, useEffect } from 'react';
import TrafficStatus from './TrafficStatus';
import XMLViewer from './XMLViewer';

interface TrafficData {
  rampId: string;
  rampName: string;
  timestamp: string;
  condition: string;
  queueLength: number;
  severity: number;
  alertRequired: boolean;
  boxAVehicles: number;
  boxBVehicles: number;
  boxCVehicles: number;
  boxAAverageStayTimeSeconds: number;
  boxBAverageStayTimeSeconds: number;
  boxCAverageStayTimeSeconds: number;
  totalVehicles: number;
}

const TrafficMonitor: React.FC = () => {
  const [rampId, setRampId] = useState('ramp-1');
  const [trafficData, setTrafficData] = useState<TrafficData | null>(null);
  const [xmlData, setXmlData] = useState<string>('');
  const [xmlIsValid, setXmlIsValid] = useState<boolean | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [autoRefresh, setAutoRefresh] = useState(false);
  const [showXml, setShowXml] = useState(false);

  // Function to fetch traffic data
  const fetchTrafficData = async () => {
    setLoading(true);
    setError(null);
    
    try {
      const response = await fetch(`http://localhost:5156/api/Traffic/status/${rampId}`);
      
      if (!response.ok) {
        throw new Error(`Error: ${response.statusText}`);
      }
      
      const data = await response.json();
      setTrafficData(data);
    } catch (err) {
      setError(`Failed to fetch traffic data: ${err instanceof Error ? err.message : String(err)}`);
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  // Function to fetch XML data
  const fetchXmlData = async () => {
    setLoading(true);
    setError(null);
    
    try {
      const response = await fetch(`http://localhost:5156/api/Traffic/datex2/${rampId}`);
      
      if (!response.ok) {
        throw new Error(`Error: ${response.statusText}`);
      }
      
      const data = await response.text();
      setXmlData(data);
      
      // Extract validation status from headers
      const validationStatus = response.headers.get('X-Datex2-Valid');
      setXmlIsValid(validationStatus === 'True');
      
      setShowXml(true);
    } catch (err) {
      setError(`Failed to fetch XML data: ${err instanceof Error ? err.message : String(err)}`);
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  // Function to validate XML separately
  const validateXml = async () => {
    if (!xmlData) return;
    
    setLoading(true);
    setError(null);
    
    try {
      const response = await fetch(`http://localhost:5156/api/Traffic/validate/${rampId}`);
      
      if (!response.ok) {
        throw new Error(`Error: ${response.statusText}`);
      }
      
      const data = await response.json();
      setXmlIsValid(data.isValid);
      
      // Show validation result message
      if (data.isValid) {
        setError(null);
      } else {
        setError(`Validation failed: ${data.message}`);
      }
    } catch (err) {
      setError(`Failed to validate XML: ${err instanceof Error ? err.message : String(err)}`);
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  // Auto-refresh effect
  useEffect(() => {
    if (autoRefresh) {
      const interval = setInterval(() => {
        fetchTrafficData();
      }, 5000); // Refresh every 5 seconds
      
      return () => clearInterval(interval);
    }
  }, [autoRefresh, rampId]);

  // Initial fetch
  useEffect(() => {
    fetchTrafficData();
  }, [rampId]);

  return (
    <div className="bg-white p-6 rounded-lg shadow-md max-w-4xl mx-auto">
      <h1 className="text-2xl font-bold mb-6 text-gray-800">Highway Exit Ramp Traffic Monitor</h1>
      
      <div className="flex mb-4 items-center">
        <label htmlFor="ramp-select" className="mr-2 font-medium">Ramp ID:</label>
        <select 
          id="ramp-select"
          className="border rounded p-2"
          value={rampId}
          onChange={(e) => setRampId(e.target.value)}
        >
          <option value="ramp-1">Highway Exit 42</option>
        </select>
        
        <div className="ml-auto flex items-center">
          <label htmlFor="auto-refresh" className="mr-2 font-medium">Auto-refresh:</label>
          <input
            id="auto-refresh"
            type="checkbox"
            checked={autoRefresh}
            onChange={() => setAutoRefresh(!autoRefresh)}
            className="h-5 w-5"
          />
        </div>
      </div>
      
      {error && (
        <div className="bg-red-100 border-l-4 border-red-500 text-red-700 p-4 mb-4" role="alert">
          <p>{error}</p>
        </div>
      )}
      
      <div className="flex space-x-4 mb-6">
        <button
          onClick={fetchTrafficData}
          disabled={loading}
          className="bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded disabled:bg-blue-300"
        >
          {loading ? 'Loading...' : 'Refresh Data'}
        </button>
        
        <button
          onClick={fetchXmlData}
          disabled={loading}
          className="bg-green-500 hover:bg-green-600 text-white px-4 py-2 rounded disabled:bg-green-300"
        >
          {loading ? 'Loading...' : 'View Datex2 XML'}
        </button>
        
        {showXml && (
          <>
            <button
              onClick={() => setShowXml(false)}
              className="bg-gray-500 hover:bg-gray-600 text-white px-4 py-2 rounded"
            >
              Hide XML
            </button>
            
            <button
              onClick={validateXml}
              disabled={loading || !xmlData}
              className="bg-purple-500 hover:bg-purple-600 text-white px-4 py-2 rounded disabled:bg-purple-300"
            >
              Validate XML
            </button>
          </>
        )}
      </div>
      
      {trafficData && !showXml && (
        <TrafficStatus data={trafficData} />
      )}
      
      {showXml && xmlData && (
        <XMLViewer xml={xmlData} isValid={xmlIsValid} />
      )}
    </div>
  );
};

export default TrafficMonitor; 
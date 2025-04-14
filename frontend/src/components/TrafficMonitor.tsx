import React, { useState, useEffect } from 'react';
import TrafficStatus from './TrafficStatus';
import XMLViewer from './XMLViewer';
import JSONViewer from './JSONViewer';

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
  const [showDataFormat, setShowDataFormat] = useState<'normal' | 'json' | 'xml' | 'both'>('normal');

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
      
      // Update view to show both formats
      setShowDataFormat('both');
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

  // Function to toggle format view
  const toggleView = (view: 'normal' | 'json' | 'xml' | 'both') => {
    setShowDataFormat(view);
    
    // If toggling to xml or both and we don't have XML data yet, fetch it
    if ((view === 'xml' || view === 'both') && !xmlData) {
      fetchXmlData();
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
    <div className="bg-white p-6 rounded-lg shadow-md max-w-6xl mx-auto">
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
          onClick={() => toggleView('normal')}
          className={`px-4 py-2 rounded ${showDataFormat === 'normal' ? 'bg-gray-700 text-white' : 'bg-gray-200 text-gray-800 hover:bg-gray-300'}`}
        >
          View Dashboard
        </button>
        
        <button
          onClick={() => toggleView('json')}
          className={`px-4 py-2 rounded ${showDataFormat === 'json' ? 'bg-yellow-500 text-white' : 'bg-yellow-100 text-yellow-800 hover:bg-yellow-200'}`}
        >
          View JSON
        </button>
        
        <button
          onClick={() => toggleView('xml')}
          className={`px-4 py-2 rounded ${showDataFormat === 'xml' ? 'bg-green-500 text-white' : 'bg-green-100 text-green-800 hover:bg-green-200'}`}
        >
          View XML
        </button>
        
        <button
          onClick={() => toggleView('both')}
          className={`px-4 py-2 rounded ${showDataFormat === 'both' ? 'bg-purple-500 text-white' : 'bg-purple-100 text-purple-800 hover:bg-purple-200'}`}
        >
          View Both
        </button>
        
        {(showDataFormat === 'xml' || showDataFormat === 'both') && (
          <button
            onClick={validateXml}
            disabled={loading || !xmlData}
            className="bg-purple-500 hover:bg-purple-600 text-white px-4 py-2 rounded disabled:bg-purple-300"
          >
            Validate XML
          </button>
        )}
      </div>
      
      {/* Dashboard View */}
      {showDataFormat === 'normal' && trafficData && (
        <TrafficStatus data={trafficData} />
      )}
      
      {/* Side-by-side JSON/XML View */}
      {showDataFormat === 'both' && trafficData && (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <h3 className="text-lg font-semibold mb-2">JSON Data (Input)</h3>
            <JSONViewer data={trafficData} />
          </div>
          <div>
            <h3 className="text-lg font-semibold mb-2">XML Data (Output)</h3>
            {xmlData ? (
              <XMLViewer xml={xmlData} isValid={xmlIsValid} />
            ) : (
              <div className="p-4 bg-gray-100 rounded">Loading XML data...</div>
            )}
          </div>
        </div>
      )}
      
      {/* JSON-only View */}
      {showDataFormat === 'json' && trafficData && (
        <div>
          <h3 className="text-lg font-semibold mb-2">JSON Data (Input)</h3>
          <JSONViewer data={trafficData} />
        </div>
      )}
      
      {/* XML-only View */}
      {showDataFormat === 'xml' && xmlData && (
        <div>
          <h3 className="text-lg font-semibold mb-2">XML Data (Output)</h3>
          <XMLViewer xml={xmlData} isValid={xmlIsValid} />
        </div>
      )}
    </div>
  );
};

export default TrafficMonitor; 
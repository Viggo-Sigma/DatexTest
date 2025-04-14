import React from 'react';

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

interface TrafficStatusProps {
  data: TrafficData;
}

const TrafficStatus: React.FC<TrafficStatusProps> = ({ data }) => {
  // Helper to determine background color based on traffic condition
  const getConditionColor = (condition: string) => {
    switch (condition) {
      case 'Normal':
        return 'bg-green-100';
      case 'SlowTraffic':
        return 'bg-yellow-100';
      case 'QueuingTraffic':
        return 'bg-orange-100';
      case 'StationaryTraffic':
        return 'bg-red-100';
      default:
        return 'bg-gray-100';
    }
  };

  // Helper to format the traffic condition text
  const formatCondition = (condition: string) => {
    switch (condition) {
      case 'Normal':
        return 'Normal Traffic';
      case 'SlowTraffic':
        return 'Slow Traffic';
      case 'QueuingTraffic':
        return 'Queuing Traffic';
      case 'StationaryTraffic':
        return 'Stationary Traffic';
      default:
        return condition;
    }
  };

  // Get formatted timestamp
  const formattedTime = new Date(data.timestamp).toLocaleString();

  return (
    <div className="space-y-6">
      <div className={`p-4 rounded-lg ${getConditionColor(data.condition)}`}>
        <div className="mb-2 flex justify-between items-center">
          <h2 className="text-xl font-bold">{data.rampName}</h2>
          <span className="text-sm text-gray-600">{formattedTime}</span>
        </div>
        
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-4">
          <div className="bg-white p-3 rounded shadow-sm">
            <div className="text-sm text-gray-500">Traffic Condition</div>
            <div className="text-lg font-semibold">{formatCondition(data.condition)}</div>
          </div>
          
          <div className="bg-white p-3 rounded shadow-sm">
            <div className="text-sm text-gray-500">Queue Length</div>
            <div className="text-lg font-semibold">{data.queueLength} meters</div>
          </div>
          
          <div className="bg-white p-3 rounded shadow-sm">
            <div className="text-sm text-gray-500">Severity</div>
            <div className="text-lg font-semibold">{data.severity}/100</div>
          </div>
          
          <div className="bg-white p-3 rounded shadow-sm">
            <div className="text-sm text-gray-500">Total Vehicles</div>
            <div className="text-lg font-semibold">{data.totalVehicles}</div>
          </div>
        </div>
        
        {data.alertRequired && (
          <div className="bg-red-50 border-l-4 border-red-500 p-4 mb-4">
            <div className="flex">
              <div className="flex-shrink-0">
                <svg className="h-5 w-5 text-red-500" viewBox="0 0 20 20" fill="currentColor">
                  <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
                </svg>
              </div>
              <div className="ml-3">
                <p className="text-sm text-red-700">
                  Alert: Traffic congestion detected. Queue extending towards highway.
                </p>
              </div>
            </div>
          </div>
        )}
      </div>
      
      <div className="bg-white p-4 rounded-lg shadow-sm">
        <h3 className="text-lg font-semibold mb-4">Traffic Box Details</h3>
        
        <div className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <BoxStats 
              name="Box A" 
              description="Furthest from highway"
              vehicles={data.boxAVehicles} 
              avgStayTime={data.boxAAverageStayTimeSeconds} 
            />
            <BoxStats 
              name="Box B" 
              description="Middle section"
              vehicles={data.boxBVehicles} 
              avgStayTime={data.boxBAverageStayTimeSeconds} 
            />
            <BoxStats 
              name="Box C" 
              description="Closest to highway"
              vehicles={data.boxCVehicles} 
              avgStayTime={data.boxCAverageStayTimeSeconds} 
              isHighlighted={data.condition === 'QueuingTraffic' || data.condition === 'StationaryTraffic'}
            />
          </div>
        </div>
      </div>
    </div>
  );
};

interface BoxStatsProps {
  name: string;
  description: string;
  vehicles: number;
  avgStayTime: number;
  isHighlighted?: boolean;
}

const BoxStats: React.FC<BoxStatsProps> = ({ name, description, vehicles, avgStayTime, isHighlighted = false }) => {
  return (
    <div className={`border rounded-lg p-3 ${isHighlighted ? 'border-red-300 bg-red-50' : 'border-gray-200'}`}>
      <div className="flex justify-between items-center mb-2">
        <h4 className="font-semibold">{name}</h4>
        <span className="text-xs text-gray-500">{description}</span>
      </div>
      
      <div className="grid grid-cols-2 gap-2">
        <div>
          <div className="text-xs text-gray-500">Vehicles</div>
          <div className="text-lg font-semibold">{vehicles}</div>
        </div>
        
        <div>
          <div className="text-xs text-gray-500">Avg. Stay Time</div>
          <div className="text-lg font-semibold">{avgStayTime.toFixed(1)}s</div>
        </div>
      </div>
    </div>
  );
};

export default TrafficStatus; 
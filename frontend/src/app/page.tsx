'use client';

import TrafficMonitor from '@/components/TrafficMonitor';

export default function Home() {
  return (
    <div className="min-h-screen bg-gray-100 p-6">
      <div className="container mx-auto py-8">
        <h1 className="text-3xl font-bold text-center mb-8 text-gray-800">
          Highway Traffic Monitoring System
        </h1>
        <p className="text-center mb-8 text-gray-600 max-w-2xl mx-auto">
          This system monitors traffic on highway exit ramps using virtual boxes (A, B, C) to detect
          congestion and alert when traffic queues build up, potentially extending back to the highway.
        </p>
        
        <TrafficMonitor />
        
        <div className="mt-12 text-center text-sm text-gray-500">
          <p>Developed using Datex2 standards for traffic information exchange</p>
        </div>
      </div>
    </div>
  );
}

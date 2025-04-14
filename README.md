# Highway Traffic Monitoring System with Datex2

This project demonstrates a traffic monitoring system that simulates tracking traffic conditions on a highway exit ramp. It uses virtual sensor boxes to detect congestion and generates alerts when traffic queues build up, potentially causing safety hazards by extending back to the highway.

## Project Structure

The project consists of:

1. **Backend (C# ASP.NET Core):**
   - Simulates traffic sensor data from three monitoring boxes (A, B, C)
   - Analyzes traffic patterns to detect congestion
   - Converts traffic data to Datex2 format (XML)
   - Exposes APIs for traffic status and Datex2 output

2. **Frontend (Next.js/React/TypeScript):**
   - Displays real-time traffic conditions
   - Shows detailed information from each monitoring box
   - Visualizes traffic status with color-coding
   - Provides Datex2 XML output viewer

3. **Datex2 Integration:**
   - Uses standard Datex2 schema files for XML validation
   - Implements traffic situation reporting according to Datex2 standards
   - Supports XML output compatible with traffic information systems

## Getting Started

### Prerequisites

- .NET 7 SDK or later
- Node.js 18 or later
- npm or yarn

### Running the Backend

1. Navigate to the backend directory:
   ```
   cd backend
   ```

2. Build and run the project:
   ```
   dotnet build
   dotnet run
   ```

3. The API should be available at `https://localhost:7234`

### Running the Frontend

1. Navigate to the frontend directory:
   ```
   cd frontend
   ```

2. Install dependencies:
   ```
   npm install
   ```

3. Start the development server:
   ```
   npm run dev
   ```

4. The frontend should be available at `http://localhost:3000`

## API Endpoints

- `GET /api/Traffic/status/{rampId}` - Get traffic status in JSON format
- `GET /api/Traffic/datex2/{rampId}` - Get traffic data in Datex2 XML format
- `GET /api/Traffic/simulate/{rampId}` - Simulate new traffic data for a ramp

## About Datex2

DATEX II is a standard for information exchange between traffic management centers, traffic information centers, service providers and media partners. It provides a standardized way to exchange and share traffic and travel information between traffic information centers, service providers, traffic operators and media partners.

This project uses Datex2 schema files for validating XML output, ensuring compatibility with other traffic management systems that support the standard.

## License

This project is licensed under the MIT License. 
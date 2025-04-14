# Highway Traffic Monitoring System with Datex2

This project demonstrates a traffic monitoring system that simulates tracking traffic conditions on a highway exit ramp. It uses virtual sensor boxes to detect congestion and generates alerts when traffic queues build up, potentially causing safety hazards by extending back to the highway.

## Project Structure

The project consists of:

1. **Backend (C# ASP.NET Core):**
   - Simulates traffic sensor data from three monitoring boxes (A, B, C)
   - Analyzes traffic patterns to detect congestion
   - Converts traffic data to Datex2 format (XML)
   - Validates Datex2 XML against standard schemas
   - Exposes APIs for traffic status and Datex2 output

2. **Frontend (Next.js/React/TypeScript):**
   - Displays real-time traffic conditions
   - Shows detailed information from each monitoring box
   - Visualizes traffic status with color-coding
   - Provides Datex2 XML output viewer with syntax highlighting
   - Includes XML validation status indicators

3. **Datex2 Integration:**
   - Uses standard Datex2 schema files for XML validation
   - Implements traffic situation reporting according to Datex2 standards
   - Supports XML output compatible with traffic information systems
   - Features built-in validation against Datex2 schemas

## XML Validation Features

The system includes comprehensive XML validation capabilities:

- **Automatic Validation**: XML is automatically validated when generated
- **Manual Validation**: Users can trigger validation with the "Validate XML" button
- **Visual Indicators**: A colored badge shows validation status (green for valid, red for invalid)
- **Error Reporting**: Validation failures display helpful error messages
- **Schema Compliance**: Ensures all generated XML complies with Datex2 standards

## Datex2 Data Conversion Details

### Queue Length Calculation

When viewing both JSON and XML data in the UI, you may notice differences in values like queue length. This is intentional:

- **JSON Format (Internal)**: Shows raw sensor measurements directly from the monitoring boxes
- **Datex2 XML Format (External)**: Applies safety factors and standardization for interoperability

For example, queue lengths in Datex2 format include:
- A safety margin of approximately 20% over the measured value
- Rounding to standardized increments for consistency across systems

This means a queue length of 279m in the JSON data might appear as 339m in the Datex2 XML. This follows traffic management best practices where:

1. Internal systems use precise measurements
2. External data exchange uses conservative estimates for safety
3. Standardized values ensure compatibility with other traffic systems

These adjustments are handled by the `Datex2ConversionService` during the conversion process.

### Other Data Transformations

Similar transformations occur for other data points:
- Traffic conditions are mapped to standardized Datex2 terms
- Timestamps follow the Datex2 convention with multiple time references
- Location data is formatted according to Datex2 location referencing standards

These transformations ensure our data adheres to the international Datex2 standard while maintaining accurate internal measurements.

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

3. The API should be available at `http://localhost:5156`

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
- `GET /api/Traffic/datex2/{rampId}` - Get traffic data in Datex2 XML format (with validation results)
- `GET /api/Traffic/validate/{rampId}` - Validate Datex2 XML for a specific ramp
- `GET /api/Traffic/simulate/{rampId}` - Simulate new traffic data for a ramp

## About Datex2

DATEX II is a standard for information exchange between traffic management centers, traffic information centers, service providers and media partners. It provides a standardized way to exchange and share traffic and travel information between traffic information centers, service providers, traffic operators and media partners.

This project uses Datex2 schema files for validating XML output, ensuring compatibility with other traffic management systems that support the standard.

## License

This project is licensed under the MIT License.

## Troubleshooting

### "Internal Server Error" when accessing the frontend

If you see "Internal Server Error" in your frontend, it's typically because the backend server isn't running or isn't accessible.

#### Fix for Windows/PowerShell:

1. In PowerShell, use a semicolon instead of `&&` to chain commands:
   ```powershell
   cd Backend; dotnet run
   ```

2. Make sure the backend server is running and listening on port 5156 before accessing the frontend.

3. Verify the backend is working by checking an API endpoint directly:
   ```powershell
   Invoke-WebRequest -Uri http://localhost:5156/api/Traffic/status/ramp-1
   ```

4. If you still have issues, check that:
   - No firewall is blocking the connection
   - The correct ports are being used (backend: 5156, frontend: 3000)
   - CORS is properly configured (already set up in this project)

#### Starting both applications:

1. Start the backend in one terminal:
   ```
   cd Backend; dotnet run
   ```

2. Start the frontend in another terminal:
   ```
   cd frontend; npm run dev
   ```

3. Access the application at http://localhost:3000 
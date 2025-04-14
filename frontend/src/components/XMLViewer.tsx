import React, { useState } from 'react';

interface XMLViewerProps {
  xml: string;
  isValid?: boolean | null;
}

const XMLViewer: React.FC<XMLViewerProps> = ({ xml, isValid }) => {
  const [copied, setCopied] = useState(false);
  
  // Format XML with syntax highlighting (basic implementation)
  const formatXml = (xml: string) => {
    // Replace special characters for HTML display
    const escapedXml = xml
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;')
      .replace(/'/g, '&apos;');
    
    // Add basic syntax highlighting with darker colors
    return escapedXml
      // Tags
      .replace(/&lt;(\/?)([\w:]+)(.*?)&gt;/g, '<span style="color: #0000CC; font-weight: bold;">&lt;$1$2</span><span style="color: #0000CC">$3</span><span style="color: #0000CC; font-weight: bold;">&gt;</span>')
      // Attributes
      .replace(/(\s+)([\w:]+)=(&quot;.*?&quot;)/g, '$1<span style="color: #CC0000; font-weight: bold;">$2</span>=<span style="color: #006600">$3</span>');
  };
  
  const copyToClipboard = () => {
    navigator.clipboard.writeText(xml).then(() => {
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    });
  };
  
  // Get validation badge color
  const getValidationBadgeColor = () => {
    if (isValid === null || isValid === undefined) return 'bg-gray-500';
    return isValid ? 'bg-green-500' : 'bg-red-500';
  };
  
  // Get validation text
  const getValidationText = () => {
    if (isValid === null || isValid === undefined) return 'Not Validated';
    return isValid ? 'Valid Datex2 XML' : 'Invalid Datex2 XML';
  };
  
  return (
    <div className="bg-gray-50 rounded-lg border border-gray-200">
      <div className="flex justify-between items-center p-3 bg-gray-100 rounded-t-lg border-b border-gray-200">
        <div className="flex items-center">
          <h3 className="font-semibold mr-2">Datex2 XML Output</h3>
          {isValid !== undefined && (
            <span className={`text-xs text-white px-2 py-1 rounded-full ${getValidationBadgeColor()}`}>
              {getValidationText()}
            </span>
          )}
        </div>
        <button
          onClick={copyToClipboard}
          className="bg-blue-500 hover:bg-blue-600 text-white text-sm px-3 py-1 rounded"
        >
          {copied ? 'Copied!' : 'Copy XML'}
        </button>
      </div>
      
      <div className="p-4 overflow-auto max-h-96">
        <pre
          className="whitespace-pre-wrap text-sm font-mono text-black"
          dangerouslySetInnerHTML={{ __html: formatXml(xml) }}
        />
      </div>
    </div>
  );
};

export default XMLViewer; 
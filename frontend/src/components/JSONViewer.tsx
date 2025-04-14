import React, { useState } from 'react';

interface JSONViewerProps {
  data: any;
}

const JSONViewer: React.FC<JSONViewerProps> = ({ data }) => {
  const [copied, setCopied] = useState(false);
  
  // Format JSON with indentation and styling
  const formatJSON = () => {
    try {
      // Convert to string if it's already an object
      const jsonString = typeof data === 'string' ? data : JSON.stringify(data, null, 2);
      
      // Syntax highlighting for JSON
      return jsonString
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)/g, 
          (match) => {
            let cls = 'color: #000000;'; // default
            if (/^"/.test(match)) {
              if (/:$/.test(match)) {
                cls = 'color: #0000CC; font-weight: bold;'; // key
              } else {
                cls = 'color: #006600;'; // string
              }
            } else if (/true|false/.test(match)) {
              cls = 'color: #CC0000;'; // boolean
            } else if (/null/.test(match)) {
              cls = 'color: #CC0000;'; // null
            } else if (/\d+/.test(match)) {
              cls = 'color: #AA00AA;'; // number
            }
            return `<span style="${cls}">${match}</span>`;
          }
        );
    } catch (e) {
      return 'Error formatting JSON';
    }
  };
  
  const copyToClipboard = () => {
    const jsonString = typeof data === 'string' ? data : JSON.stringify(data, null, 2);
    navigator.clipboard.writeText(jsonString).then(() => {
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    });
  };
  
  return (
    <div className="bg-gray-50 rounded-lg border border-gray-200">
      <div className="flex justify-between items-center p-3 bg-gray-100 rounded-t-lg border-b border-gray-200">
        <h3 className="font-semibold">JSON Data</h3>
        <button
          onClick={copyToClipboard}
          className="bg-blue-500 hover:bg-blue-600 text-white text-sm px-3 py-1 rounded"
        >
          {copied ? 'Copied!' : 'Copy JSON'}
        </button>
      </div>
      
      <div className="p-4 overflow-auto max-h-96">
        <pre
          className="whitespace-pre-wrap text-sm font-mono text-black"
          dangerouslySetInnerHTML={{ __html: formatJSON() }}
        />
      </div>
    </div>
  );
};

export default JSONViewer; 
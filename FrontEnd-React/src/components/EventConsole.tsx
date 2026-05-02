import { useState, useEffect, useRef } from 'react';

interface EventConsoleProps {
  messages: string[];
}

export default function EventConsole({ messages }: EventConsoleProps) {
  const [open, setOpen] = useState(false);
  const bottomRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (open && bottomRef.current) {
      bottomRef.current.scrollIntoView({ behavior: 'smooth' });
    }
  }, [messages, open]);

  return (
    <div
      style={{
        position: 'fixed',
        bottom: '12px',
        left: '12px',
        zIndex: 1000,
        width: '320px',
        fontFamily: 'monospace',
        fontSize: '11px',
      }}
    >
      <details open={open} onToggle={(e) => setOpen((e.target as HTMLDetailsElement).open)}>
        <summary
          style={{
            cursor: 'pointer',
            backgroundColor: '#1e1e1e',
            color: '#ccc',
            padding: '4px 8px',
            borderRadius: open ? '4px 4px 0 0' : '4px',
            userSelect: 'none',
          }}
        >
          Event Console ({messages.length})
        </summary>
        <div
          style={{
            backgroundColor: '#1e1e1e',
            color: '#00ff88',
            padding: '6px 8px',
            borderRadius: '0 0 4px 4px',
            maxHeight: '160px',
            overflowY: 'auto',
          }}
        >
          {messages.length === 0 && (
            <span style={{ color: '#666' }}>No events yet.</span>
          )}
          {messages.map((msg, i) => (
            <div key={i}>{msg}</div>
          ))}
          <div ref={bottomRef} />
        </div>
      </details>
    </div>
  );
}

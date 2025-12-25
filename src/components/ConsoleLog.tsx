import React, { useEffect, useRef } from 'react';
import { Terminal, XCircle, CheckCircle, Info } from 'lucide-react';
import clsx from 'clsx';
import { useTranslation } from 'react-i18next';

interface LogEntry {
    id: string;
    timestamp: string;
    type: 'info' | 'success' | 'error' | 'warning';
    message: string;
}

interface ConsoleLogProps {
    logs: LogEntry[];
}

const ConsoleLog: React.FC<ConsoleLogProps> = ({ logs }) => {
    const endRef = useRef<HTMLDivElement>(null);
    const { t } = useTranslation();

    useEffect(() => {
        endRef.current?.scrollIntoView({ behavior: 'smooth' });
    }, [logs]);

    return (
        <div className="flex flex-col h-64 bg-black/90 rounded-lg border border-border overflow-hidden font-mono text-xs shadow-2xl">
            <div className="flex items-center justify-between px-3 py-1 bg-muted/20 border-b border-border/50">
                <div className="flex items-center gap-2 text-muted-foreground">
                    <Terminal className="h-3 w-3" />
                    <span>{t('console.title')}</span>
                </div>
                <div className="flex gap-1.5">
                    <div className="h-2 w-2 rounded-full bg-red-500/50"></div>
                    <div className="h-2 w-2 rounded-full bg-yellow-500/50"></div>
                    <div className="h-2 w-2 rounded-full bg-green-500/50"></div>
                </div>
            </div>

            <div className="flex-1 overflow-auto p-3 space-y-1 scrollbar-thin scrollbar-thumb-border scrollbar-track-transparent">
                {logs.length === 0 && (
                    <div className="text-muted-foreground/50 italic">{t('console.ready')}</div>
                )}
                {logs.map((log) => (
                    <div key={log.id} className="flex gap-2 items-start animate-in fade-in slide-in-from-left-1 duration-200">
                        <span className="text-muted-foreground shrink-0">[{log.timestamp}]</span>
                        <span className={clsx(
                            "flex items-center gap-1.5 break-all",
                            log.type === 'info' && "text-blue-400",
                            log.type === 'success' && "text-green-400",
                            log.type === 'error' && "text-red-400",
                            log.type === 'warning' && "text-yellow-400"
                        )}>
                            {log.type === 'success' && <CheckCircle className="h-3 w-3 inline" />}
                            {log.type === 'error' && <XCircle className="h-3 w-3 inline" />}
                            {log.type === 'info' && <Info className="h-3 w-3 inline" />}
                            {log.message}
                        </span>
                    </div>
                ))}
                <div ref={endRef} />
            </div>
        </div>
    );
};

export default ConsoleLog;

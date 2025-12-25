import React, { createContext, useContext, useState, type ReactNode } from 'react';

interface Device {
    name: string;
    model: string;
    imei: string;
    serial: string;
    androidVersion: string;
    batteryLevel: number;
    isConnected: boolean;
}

interface LogEntry {
    id: string;
    timestamp: string;
    type: 'info' | 'success' | 'error' | 'warning';
    message: string;
}

interface DeviceContextType {
    device: Device | null;
    logs: LogEntry[];
    connectDevice: () => void;
    disconnectDevice: () => void;
    addLog: (type: LogEntry['type'], message: string) => void;
    isBusy: boolean;
    setBusy: (busy: boolean) => void;
}

const DeviceContext = createContext<DeviceContextType | undefined>(undefined);

export const DeviceProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const [device, setDevice] = useState<Device | null>(null);
    const [logs, setLogs] = useState<LogEntry[]>([
        { id: '1', timestamp: new Date().toLocaleTimeString(), type: 'info', message: 'Application started successfully' },
        { id: '2', timestamp: new Date().toLocaleTimeString(), type: 'info', message: 'Connected to update server' },
        { id: '3', timestamp: new Date().toLocaleTimeString(), type: 'success', message: 'Database synchronized (v2025.12.10)' },
    ]);
    const [isBusy, setBusy] = useState(false);

    const addLog = (type: LogEntry['type'], message: string) => {
        setLogs(prev => [...prev, {
            id: Date.now().toString() + Math.random(),
            timestamp: new Date().toLocaleTimeString(),
            type,
            message
        }]);
    };

    const connectDevice = () => {
        if (isBusy) return;
        setBusy(true);
        addLog('info', 'Searching for devices via USB...');

        setTimeout(() => {
            setDevice({
                name: 'Samsung Galaxy S24 Ultra',
                model: 'SM-S928B',
                imei: '354829104829102',
                serial: 'R5CT10...',
                androidVersion: '14.0 (OneUI 6.1)',
                batteryLevel: 85,
                isConnected: true
            });
            addLog('success', 'Device found: Samsung Galaxy S24 Ultra (SM-S928B)');
            addLog('info', 'Reading device information...');
            setTimeout(() => {
                addLog('success', 'Device information read successfully');
                setBusy(false);
            }, 500);
        }, 1500);
    };

    const disconnectDevice = () => {
        setDevice(null);
        addLog('warning', 'Device disconnected');
    };

    return (
        <DeviceContext.Provider value={{ device, logs, connectDevice, disconnectDevice, addLog, isBusy, setBusy }}>
            {children}
        </DeviceContext.Provider>
    );
};

export const useDevice = () => {
    const context = useContext(DeviceContext);
    if (context === undefined) {
        throw new Error('useDevice must be used within a DeviceProvider');
    }
    return context;
};

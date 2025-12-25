import React, { useState } from 'react';
import {
    Smartphone, Cpu, Zap, Lock, Unlock,
    Shield, Database, FileCode, Settings, Wifi, HardDrive,
    RotateCcw, Key, Activity
} from 'lucide-react';
import ConsoleLog from '../components/ConsoleLog';
import FeaturePanel from '../components/FeaturePanel';
import { useDevice } from '../context/DeviceContext';
import { useTranslation } from 'react-i18next';
import clsx from 'clsx';

const Dashboard: React.FC = () => {
    const { device, logs, connectDevice, disconnectDevice, addLog, isBusy } = useDevice();
    const { t } = useTranslation();
    const [activeCategory, setActiveCategory] = useState('unlock');

    const runOperation = (name: string, steps: string[]) => {
        if (!device || isBusy) return;
        addLog('info', `Starting operation: ${name}...`);

        let stepIndex = 0;
        const interval = setInterval(() => {
            if (stepIndex >= steps.length) {
                clearInterval(interval);
                addLog('success', `Operation ${name} completed successfully.`);
                return;
            }
            addLog('info', steps[stepIndex]);
            stepIndex++;
        }, 1500);
    };

    const categories = [
        { id: 'unlock', label: 'Unlock', icon: Unlock },
        { id: 'repair', label: 'Repair', icon: Wrench }, // Wrench not imported, fixing below
        { id: 'software', label: 'Software', icon: FileCode },
        { id: 'system', label: 'System', icon: Settings },
        { id: 'backup', label: 'Backup', icon: Database },
    ];

    return (
        <div className="space-y-6">
            {/* Device Status Area */}
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                <div className="lg:col-span-3 bg-card rounded-xl border border-border p-6 shadow-lg relative overflow-hidden group">
                    <div className="absolute top-0 right-0 p-4 opacity-10 group-hover:opacity-20 transition-opacity">
                        <Smartphone className="h-48 w-48 text-primary" />
                    </div>

                    <div className="flex items-center justify-between mb-6">
                        <h2 className="text-xl font-bold flex items-center gap-2">
                            <Smartphone className="h-5 w-5 text-primary" />
                            {t('dashboard.deviceStatus')}
                        </h2>
                        {device && (
                            <div className="flex gap-2">
                                <span className="px-3 py-1 bg-green-500/10 text-green-500 text-xs font-bold rounded border border-green-500/20 flex items-center gap-1">
                                    <Wifi className="h-3 w-3" /> COM3 (High Speed)
                                </span>
                                <span className="px-3 py-1 bg-blue-500/10 text-blue-500 text-xs font-bold rounded border border-blue-500/20 flex items-center gap-1">
                                    <Zap className="h-3 w-3" /> ADB Mode
                                </span>
                            </div>
                        )}
                    </div>

                    {!device ? (
                        <div className="h-32 flex items-center justify-center border-2 border-dashed border-border rounded-lg bg-muted/10">
                            <div className="text-center">
                                <p className="text-muted-foreground font-medium mb-2">{t('dashboard.waiting')}</p>
                                <button
                                    onClick={connectDevice}
                                    disabled={isBusy}
                                    className="px-6 py-2 bg-primary text-primary-foreground rounded-full hover:bg-primary/90 transition-colors text-sm font-bold shadow-lg shadow-primary/20"
                                >
                                    {isBusy ? t('dashboard.connecting') : t('dashboard.simulate')}
                                </button>
                            </div>
                        </div>
                    ) : (
                        <div className="flex flex-col md:flex-row gap-6 relative z-10">
                            <div className="flex items-center gap-4">
                                <div className="h-24 w-24 bg-black rounded-xl border border-border flex items-center justify-center p-2">
                                    <img src="https://placehold.co/100x100/000000/FFFFFF/png?text=S24" alt="Device" className="h-full object-contain" />
                                </div>
                                <div>
                                    <h3 className="text-2xl font-bold text-white">{device.name}</h3>
                                    <p className="text-primary font-mono text-sm mb-1">{device.model}</p>
                                    <p className="text-xs text-muted-foreground">CPID: <span className="font-mono text-foreground">A309</span> • SW: <span className="font-mono text-foreground">{device.androidVersion}</span></p>
                                </div>
                            </div>

                            <div className="flex-1 grid grid-cols-2 md:grid-cols-4 gap-3">
                                <div className="p-3 bg-muted/20 rounded border border-border/50">
                                    <span className="text-xs text-muted-foreground block">{t('dashboard.imei')}</span>
                                    <span className="font-mono text-sm font-bold">{device.imei}</span>
                                </div>
                                <div className="p-3 bg-muted/20 rounded border border-border/50">
                                    <span className="text-xs text-muted-foreground block">{t('dashboard.serial')}</span>
                                    <span className="font-mono text-sm font-bold">{device.serial}</span>
                                </div>
                                <div className="p-3 bg-muted/20 rounded border border-border/50">
                                    <span className="text-xs text-muted-foreground block">Knox Status</span>
                                    <span className="font-mono text-sm font-bold text-green-400">0x0 (Valid)</span>
                                </div>
                                <div className="p-3 bg-muted/20 rounded border border-border/50">
                                    <span className="text-xs text-muted-foreground block">{t('dashboard.battery')}</span>
                                    <span className="font-mono text-sm font-bold text-green-400">{device.batteryLevel}%</span>
                                </div>
                            </div>

                            <button
                                onClick={disconnectDevice}
                                className="absolute top-0 right-0 text-xs text-red-400 hover:text-red-300 underline"
                            >
                                {t('dashboard.disconnect')}
                            </button>
                        </div>
                    )}
                </div>
            </div>

            {/* Feature Tabs */}
            <div className="flex overflow-x-auto gap-2 pb-2 border-b border-border">
                {categories.map(cat => (
                    <button
                        key={cat.id}
                        onClick={() => setActiveCategory(cat.id)}
                        className={clsx(
                            "flex items-center gap-2 px-4 py-2 rounded-t-lg font-medium text-sm transition-all whitespace-nowrap",
                            activeCategory === cat.id
                                ? "bg-primary text-primary-foreground"
                                : "bg-card hover:bg-muted text-muted-foreground"
                        )}
                    >
                        <cat.icon className="h-4 w-4" />
                        {cat.label}
                    </button>
                ))}
            </div>

            {/* Feature Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 min-h-[300px]">
                {activeCategory === 'unlock' && (
                    <>
                        <FeaturePanel
                            title={t('dashboard.networkUnlock')}
                            description={t('dashboard.networkUnlockDesc')}
                            icon={Unlock}
                            actionLabel="Start Unlock"
                            onAction={() => runOperation('Network Unlock', [
                                t('console.checkingRoot'),
                                t('console.calcCodes'),
                                t('console.writing'),
                                'Verifying lock status...',
                                'Patching modem firmware...'
                            ])}
                            disabled={!device}
                        />
                        <FeaturePanel
                            title={t('dashboard.frpReset')}
                            description={t('dashboard.frpResetDesc')}
                            icon={Lock}
                            actionLabel="Reset FRP"
                            onAction={() => runOperation('FRP Reset', [
                                'Reading partition table...',
                                'Locating persistent partition...',
                                'Erasing FRP data...',
                                'Rebooting device...'
                            ])}
                            disabled={!device}
                        />
                        <FeaturePanel
                            title="Bootloader Unlock"
                            description="Unlock device bootloader for custom ROMs"
                            icon={Shield}
                            actionLabel="Unlock BL"
                            warning="Trips Knox Warranty"
                            onAction={() => runOperation('Bootloader Unlock', [
                                'Checking OEM Unlock status...',
                                'Generating unlock token...',
                                'Flashing unlock token...',
                                'Device will wipe data!'
                            ])}
                            disabled={!device}
                        />
                        <FeaturePanel
                            title="KG / RMM Unlock"
                            description="Remove regional lock mechanisms"
                            icon={Activity}
                            actionLabel="Bypass"
                            onAction={() => runOperation('KG/RMM Bypass', [
                                'Exploiting LPE...',
                                'Patching vaultkeeper...',
                                'Success!'
                            ])}
                            disabled={!device}
                        />
                    </>
                )}

                {activeCategory === 'repair' && (
                    <>
                        <FeaturePanel
                            title="Repair IMEI"
                            description="Restore original IMEI numbers"
                            icon={Hash} // Fixing import below
                            actionLabel="Repair"
                            onAction={() => runOperation('IMEI Repair', [
                                'Backing up EFS...',
                                'Writing new IMEI A...',
                                'Writing new IMEI B...',
                                'Patching certificate...'
                            ])}
                            disabled={!device}
                        />
                        <FeaturePanel
                            title="Fix SN / MAC"
                            description="Repair Serial No, WiFi/BT MAC"
                            icon={Wifi}
                            actionLabel="Fix"
                            onAction={() => runOperation('SN/MAC Fix', [
                                'Reading param partition...',
                                'Updating MAC address...',
                                'Writing new Serial Number...'
                            ])}
                            disabled={!device}
                        />
                    </>
                )}

                {activeCategory === 'software' && (
                    <>
                        <FeaturePanel
                            title={t('dashboard.flashFirmware')}
                            description={t('dashboard.flashFirmwareDesc')}
                            icon={FileCode}
                            actionLabel="Flash"
                            onAction={() => runOperation('Firmware Flash', [
                                'Analyzing firmware package...',
                                'Rebooting to Download Mode...',
                                'Flashing BOOT...',
                                'Flashing SYSTEM...',
                                'Flashing VENDOR...',
                                'Flashing USERDATA...'
                            ])}
                            disabled={!device}
                        />
                        <FeaturePanel
                            title="Root Device"
                            description="Install Magisk / SuperSU"
                            icon={Cpu}
                            actionLabel="Root"
                            onAction={() => runOperation('Root', [
                                'Patching boot image...',
                                'Flashing patched boot...',
                                'Installing Magisk stub...'
                            ])}
                            disabled={!device}
                        />
                    </>
                )}

                {activeCategory === 'system' && (
                    <>
                        <FeaturePanel
                            title="Screen Lock Removal"
                            description="Remove PIN/Pattern without data loss"
                            icon={Key}
                            actionLabel="Remove"
                            onAction={() => runOperation('Screen Lock Removal', [
                                'Mounting /data...',
                                'Deleting locksettings.db...',
                                'Removing gatekeeper.pattern.key...',
                                'Done!'
                            ])}
                            disabled={!device}
                        />
                        <FeaturePanel
                            title="Factory Reset"
                            description="Wipe all user data"
                            icon={RotateCcw}
                            actionLabel="Reset"
                            warning="Data will be lost"
                            onAction={() => runOperation('Factory Reset', [
                                'Formatting /data...',
                                'Formatting /cache...',
                                'Rebooting...'
                            ])}
                            disabled={!device}
                        />
                    </>
                )}

                {activeCategory === 'backup' && (
                    <>
                        <FeaturePanel
                            title="Full Backup"
                            description="Backup all partitions"
                            icon={HardDrive}
                            actionLabel="Backup"
                            onAction={() => runOperation('Full Backup', [
                                'Reading partition table...',
                                'Backing up EFS (Critical)...',
                                'Backing up Modem...',
                                'Backing up System (Large)...'
                            ])}
                            disabled={!device}
                        />
                        <FeaturePanel
                            title="Read Cert"
                            description="Read cryptographic certificates"
                            icon={FileCode}
                            actionLabel="Read"
                            onAction={() => runOperation('Read Cert', [
                                'Authenticating with modem...',
                                'Exploiting diag port...',
                                'Reading KEY_ID...',
                                'Saving cert file...'
                            ])}
                            disabled={!device}
                        />
                    </>
                )}
            </div>

            {/* Console Log */}
            <ConsoleLog logs={logs} />
        </div>
    );
};

// Helper for icons not in the main import list to avoid clutter
import { Wrench, Hash } from 'lucide-react';

export default Dashboard;

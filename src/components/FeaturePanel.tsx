import React from 'react';
import { Play, AlertTriangle } from 'lucide-react';
import { useDevice } from '../context/DeviceContext';

interface FeaturePanelProps {
    title: string;
    description: string;
    icon: React.ElementType;
    actionLabel: string;
    onAction: () => void;
    warning?: string;
    disabled?: boolean;
}

const FeaturePanel: React.FC<FeaturePanelProps> = ({
    title,
    description,
    icon: Icon,
    actionLabel,
    onAction,
    warning,
    disabled
}) => {
    const { isBusy } = useDevice();

    return (
        <div className="bg-card border border-border rounded-xl p-5 flex flex-col h-full hover:border-primary/50 transition-colors group">
            <div className="flex items-start justify-between mb-4">
                <div className="p-3 bg-primary/10 rounded-lg group-hover:bg-primary/20 transition-colors">
                    <Icon className="h-6 w-6 text-primary" />
                </div>
                {warning && (
                    <div className="text-yellow-500" title={warning}>
                        <AlertTriangle className="h-5 w-5" />
                    </div>
                )}
            </div>

            <h3 className="text-lg font-bold mb-2">{title}</h3>
            <p className="text-sm text-muted-foreground mb-6 flex-1">{description}</p>

            <button
                onClick={onAction}
                disabled={disabled || isBusy}
                className="w-full py-2 px-4 bg-secondary hover:bg-secondary/80 text-secondary-foreground rounded-lg font-medium flex items-center justify-center gap-2 transition-all disabled:opacity-50 disabled:cursor-not-allowed active:scale-95"
            >
                <Play className="h-4 w-4 fill-current" />
                {actionLabel}
            </button>
        </div>
    );
};

export default FeaturePanel;

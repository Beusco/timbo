import React, { useState } from 'react';
import { Shield, User, Lock, ArrowRight, AlertCircle } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { useTranslation } from 'react-i18next';

const Login: React.FC = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const { login } = useAuth();
    const { t } = useTranslation();

    const handleLogin = (e: React.FormEvent) => {
        e.preventDefault();
        setError('');

        if (!username || !password) {
            setError('Username and password are required');
            return;
        }

        try {
            login(username, password);
        } catch (err: any) {
            setError(err.message);
        }
    };

    return (
        <div className="min-h-screen w-full flex items-center justify-center bg-background relative overflow-hidden">
            {/* Background Elements */}
            <div className="absolute top-0 left-0 w-full h-full overflow-hidden z-0">
                <div className="absolute top-[-10%] left-[-10%] w-[40%] h-[40%] rounded-full bg-primary/5 blur-[100px]"></div>
                <div className="absolute bottom-[-10%] right-[-10%] w-[40%] h-[40%] rounded-full bg-secondary/10 blur-[100px]"></div>
            </div>

            <div className="w-full max-w-md p-8 bg-card/50 backdrop-blur-xl border border-border rounded-2xl shadow-2xl z-10 relative">
                <div className="flex flex-col items-center mb-8">
                    <div className="h-16 w-16 rounded-2xl bg-primary flex items-center justify-center mb-4 shadow-lg shadow-primary/20">
                        <Shield className="h-8 w-8 text-primary-foreground" />
                    </div>
                    <h1 className="text-3xl font-bold tracking-tight text-center">
                        {t('app.title')}<span className="text-primary">{t('app.subtitle')}</span>
                    </h1>
                    <p className="text-muted-foreground text-sm mt-2">Professional Service Platform</p>
                </div>

                <form onSubmit={handleLogin} className="space-y-4">
                    {error && (
                        <div className="p-3 rounded-lg bg-destructive/10 border border-destructive/20 flex items-center gap-2 text-sm text-destructive">
                            <AlertCircle className="h-4 w-4" />
                            {error}
                        </div>
                    )}

                    <div className="space-y-2">
                        <label className="text-sm font-medium text-muted-foreground flex items-center gap-2">
                            <User className="h-4 w-4" /> Username
                        </label>
                        <input
                            type="text"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            className="w-full px-4 py-3 rounded-lg bg-background/50 border border-border focus:border-primary focus:ring-1 focus:ring-primary outline-none transition-all"
                            placeholder="Enter username"
                        />
                    </div>

                    <div className="space-y-2">
                        <label className="text-sm font-medium text-muted-foreground flex items-center gap-2">
                            <Lock className="h-4 w-4" /> Password
                        </label>
                        <input
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            className="w-full px-4 py-3 rounded-lg bg-background/50 border border-border focus:border-primary focus:ring-1 focus:ring-primary outline-none transition-all"
                            placeholder="Enter password"
                        />
                    </div>

                    <button
                        type="submit"
                        className="w-full mt-6 py-3 px-4 bg-primary text-primary-foreground rounded-lg font-bold hover:bg-primary/90 transition-all flex items-center justify-center gap-2 group"
                    >
                        Login
                        <ArrowRight className="h-4 w-4 group-hover:translate-x-1 transition-transform" />
                    </button>
                </form>

                <div className="mt-6 text-center text-xs text-muted-foreground">
                    <p>Secure Connection • v3.0.0-beta</p>
                </div>
            </div>
        </div>
    );
};

export default Login;

import React from 'react';
import {
    Smartphone,
    Settings,
    Download,
    User,
    CreditCard,
    Terminal,
    Activity,
    Shield,
    Languages,
    LogOut
} from 'lucide-react';
import clsx from 'clsx';
import { useTranslation } from 'react-i18next';
import { NavLink, Outlet } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const Layout: React.FC = () => {
    const { t, i18n } = useTranslation();
    const { user, credits, logout } = useAuth();

    const navItems = [
        { id: 'dashboard', path: '/', icon: Activity, label: t('sidebar.dashboard') },
        { id: 'devices', path: '/devices', icon: Smartphone, label: t('sidebar.devices') },
        { id: 'downloads', path: '/downloads', icon: Download, label: t('sidebar.downloads') },
        { id: 'console', path: '/console', icon: Terminal, label: t('sidebar.console') },
        { id: 'settings', path: '/settings', icon: Settings, label: t('sidebar.settings') },
    ];

    const toggleLanguage = () => {
        const newLang = i18n.language === 'en' ? 'fr' : 'en';
        i18n.changeLanguage(newLang);
    };

    return (
        <div className="flex h-screen w-full bg-background text-foreground overflow-hidden font-sans selection:bg-primary selection:text-primary-foreground">
            {/* Sidebar */}
            <aside className="w-64 border-r border-border bg-card/50 backdrop-blur-xl flex flex-col">
                <div className="p-6 border-b border-border flex items-center gap-3">
                    <div className="h-8 w-8 rounded bg-primary flex items-center justify-center">
                        <Shield className="h-5 w-5 text-primary-foreground" />
                    </div>
                    <span className="text-xl font-bold tracking-tight">{t('app.title')}<span className="text-primary">{t('app.subtitle')}</span></span>
                </div>

                <nav className="flex-1 p-4 space-y-1">
                    {navItems.map((item) => (
                        <NavLink
                            key={item.id}
                            to={item.path}
                            className={({ isActive }) => clsx(
                                "w-full flex items-center gap-3 px-4 py-3 rounded-md text-sm font-medium transition-all duration-200",
                                isActive
                                    ? "bg-primary/10 text-primary border-l-2 border-primary"
                                    : "text-muted-foreground hover:bg-muted hover:text-foreground"
                            )}
                        >
                            <item.icon className="h-5 w-5" />
                            {item.label}
                        </NavLink>
                    ))}
                </nav>

                <div className="p-4 border-t border-border bg-muted/20">
                    <div className="flex items-center gap-3 mb-3">
                        <div className="h-10 w-10 rounded-full bg-secondary flex items-center justify-center">
                            <User className="h-5 w-5 text-secondary-foreground" />
                        </div>
                        <div className="flex-1 min-w-0">
                            <p className="text-sm font-medium truncate">{user?.username || t('sidebar.admin')}</p>
                            <p className="text-xs text-muted-foreground">{t('sidebar.license')}</p>
                        </div>
                        <button onClick={logout} className="text-muted-foreground hover:text-destructive transition-colors" title="Logout">
                            <LogOut className="h-4 w-4" />
                        </button>
                    </div>
                    <div className="flex items-center justify-between text-xs text-muted-foreground bg-background/50 p-2 rounded border border-border">
                        <span className="flex items-center gap-1"><CreditCard className="h-3 w-3" /> {t('sidebar.credits')}:</span>
                        <span className={clsx("font-bold", credits < 500 ? "text-red-500" : "text-primary")}>
                            {credits.toLocaleString()}
                        </span>
                    </div>
                </div>
            </aside>

            {/* Main Content */}
            <main className="flex-1 flex flex-col min-w-0 bg-gradient-to-br from-background to-secondary/5">
                {/* Top Bar */}
                <header className="h-16 border-b border-border bg-card/30 backdrop-blur-md flex items-center justify-between px-6">
                    <div className="flex items-center gap-2 text-sm text-muted-foreground">
                        <span className="h-2 w-2 rounded-full bg-green-500 animate-pulse"></span>
                        {t('app.status')}: <span className="text-green-500 font-medium">{t('app.online')}</span>
                    </div>
                    <div className="flex items-center gap-4">
                        <button
                            onClick={toggleLanguage}
                            className="flex items-center gap-2 px-3 py-1.5 rounded-full bg-muted/30 hover:bg-muted/50 transition-colors text-xs font-medium border border-border"
                        >
                            <Languages className="h-3.5 w-3.5" />
                            {i18n.language === 'en' ? 'English' : 'Français'}
                        </button>
                        <span className="text-xs text-muted-foreground">{t('app.version')}</span>
                    </div>
                </header>

                {/* Page Content */}
                <div className="flex-1 overflow-auto p-6 scrollbar-thin scrollbar-thumb-border scrollbar-track-transparent">
                    <Outlet />
                </div>
            </main>
        </div>
    );
};

export default Layout;

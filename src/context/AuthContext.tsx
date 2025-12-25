import React, { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import { userConfig } from '../config/userConfig';

interface User {
    username: string;
}

interface AuthContextType {
    user: User | null;
    credits: number;
    login: (username: string, password?: string) => void;
    logout: () => void;
    isAuthenticated: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const [user, setUser] = useState<User | null>(null);
    const [credits, setCredits] = useState<number>(() => {
        const stored = localStorage.getItem('timbo_credits');
        return stored ? parseInt(stored, 10) : userConfig.defaultCredits;
    });

    useEffect(() => {
        localStorage.setItem('timbo_credits', credits.toString());
    }, [credits]);

    const login = (usernameInput: string, passwordInput?: string) => {
        // Validate credentials
        if (usernameInput !== userConfig.username || passwordInput !== userConfig.password) {
            throw new Error('Invalid username or password');
        }

        // Check credits
        if (credits < userConfig.minCreditsToLogin) {
            throw new Error(`Account Blocked: Insufficient credits (${credits}). Contact Admin.`);
        }

        // Deduct credits
        setCredits(prev => prev - userConfig.loginCost);
        setUser({ username: userConfig.username });
    };

    const logout = () => {
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{ user, credits, login, logout, isAuthenticated: !!user }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (context === undefined) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};

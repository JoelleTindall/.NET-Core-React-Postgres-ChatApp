import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import { fileURLToPath, URL } from 'node:url';

export default defineConfig({
    plugins: [react()],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    server: {
        proxy: {
            '^/weatherforecast': {
                target: 'http://server:5000', // Use Docker service name
                secure: false,
                changeOrigin: true
            },
            '^/chat': {
                target: 'http://server:5000',
                secure: false,
                changeOrigin: true
            },
            '^/avatar': {
                target: 'http://server:5000',
                secure: false,
                changeOrigin: true
            },
            '^/account/login': {
                target: 'http://server:5000',
                secure: false,
                changeOrigin: true
            },
            '^/account/logout': {
                target: 'http://server:5000',
                secure: false,
                changeOrigin: true
            },
            '^/account/register': {
                target: 'http://server:5000',
                secure: false,
                changeOrigin: true
            },
            '^/account/me': {
                target: 'http://server:5000',
                secure: false,
                changeOrigin: true
            },

            '^/chathub': {
                target: 'http://server:5000',
                ws: true,
                secure: false,
                changeOrigin: true
            }
        },
        host: '0.0.0.0',
        port: 3000,
        strictPort: true
    }
});
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
            '/api/weatherforecast': {
                target: 'http://server:5000', // Use Docker service name
                secure: false,
                changeOrigin: true
            },
            '/api/chat': {
                target: 'http://server:5000',
                secure: false,
                changeOrigin: true
            },
            '/api/avatar': {
                target: 'http://server:5000',
                secure: false,
                changeOrigin: true
            },
            '/api/account/': {
                target: 'http://server:5000',
                secure: false,
                changeOrigin: true
            },
            '/api/chathub': {
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
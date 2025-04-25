import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import { fileURLToPath, URL } from 'node:url';

export default defineConfig(({ mode }) => {
    const isDev = mode === 'development';
    const targetHost = isDev ? 'http://localhost:8080' : 'http://server:8080';

    return {
        plugins: [react()],
        resolve: {
            alias: {
                '@': fileURLToPath(new URL('./src', import.meta.url))
            }
        },
        server: {
            proxy: {
                '/api/chat': {
                    target: targetHost,
                    secure: false,
                    changeOrigin: true
                },
                '/api/avatar': {
                    target: targetHost,
                    secure: false,
                    changeOrigin: true
                },
                '/api/register': {
                    target: targetHost,
                    secure: false,
                    changeOrigin: true
                },
                '/api/login': {
                    target: targetHost,
                    secure: false,
                    changeOrigin: true
                },
                '/api/logout': {
                    target: targetHost,
                    secure: false,
                    changeOrigin: true
                },
                '/api/chathub': {
                    target: targetHost,
                    ws: true,
                    secure: false,
                    changeOrigin: true
                }
            },
            host: '0.0.0.0',
            port: 3000,
            strictPort: true
        }
    };
});

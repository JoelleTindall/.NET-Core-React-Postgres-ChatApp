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
        target: 'http://server:8080', // Use Docker service name
        secure: false,
        changeOrigin: true
      },
      '^/chat': {
        target: 'http://server:8080',
        secure: false,
        changeOrigin: true
      },
      '^/avatar': {
        target: 'http://server:8080',
        secure: false,
        changeOrigin: true
      },
    //   '^/pingauth': {
    //     target: 'http://server:8080',
    //     secure: false,
    //     changeOrigin: true
    //   },
      '^/register': {
        target: 'http://server:8080',
        secure: false,
        changeOrigin: true
      },
      '^/login': {
        target: 'http://server:8080',
        secure: false,
        changeOrigin: true
      },
      '^/logout': {
        target: 'http://server:8080',
        secure: false,
        changeOrigin: true
      },
      '^/chathub': {
        target: 'http://server:8080',
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
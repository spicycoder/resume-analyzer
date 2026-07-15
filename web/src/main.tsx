import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { ClarityScript } from '@/components/ClarityScript'
import './index.css'
import App from './App'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <ClarityScript />
    <App />
  </StrictMode>,
)

if (import.meta.env.PROD && 'serviceWorker' in navigator) {
  window.addEventListener('load', () =>
    navigator.serviceWorker.register(`${import.meta.env.BASE_URL}sw.js`).catch(() => {}),
  )
}

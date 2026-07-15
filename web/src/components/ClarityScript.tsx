import { useEffect } from 'react'

declare global {
  interface Window {
    clarity: (method: string, ...args: (string | undefined)[]) => void
  }
}

export function ClarityScript() {
  const clarityId = import.meta.env.VITE_CLARITY_ID

  useEffect(() => {
    if (!clarityId) return

    const s = document.createElement('script')
    s.innerHTML = `(function(c,l,a,r,i,t,y){c[a]=c[a]||function(){(c[a].q=c[a].q||[]).push(arguments)};t=l.createElement(r);t.async=1;t.src="https://www.clarity.ms/tag/"+i;y=l.getElementsByTagName(r)[0];y.parentNode.insertBefore(t,y);})(window,document,"clarity","script","${clarityId}");`
    document.head.appendChild(s)
    return () => s.remove()
  }, [clarityId])

  return null
}

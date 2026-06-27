import { useState, useRef } from "react"
import { Sparkles, Upload, X, FileText, AlertCircle, RefreshCw } from "lucide-react"

import Orb from "@/components/react-bits/Orb/Orb"
import ShinyText from "@/components/react-bits/ShinyText/ShinyText"
import BorderGlow from "@/components/react-bits/BorderGlow/BorderGlow"
import RotatingText from "@/components/react-bits/RotatingText/RotatingText"
import { Button } from "@/components/ui/button"
import ResultContent, { type AnalysisResult } from "@/components/ResultContent/ResultContent"

interface FileEntry {
  file: File | null
  name: string
}

export default function HomePage() {
  const [jd, setJd] = useState<FileEntry>({ file: null, name: "" })
  const [resume, setResume] = useState<FileEntry>({ file: null, name: "" })
  const [result, setResult] = useState<AnalysisResult | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)
  const jdRef = useRef<HTMLInputElement>(null)
  const resumeRef = useRef<HTMLInputElement>(null)

  const handleFile = (field: "jd" | "resume") => (e: React.ChangeEvent<HTMLInputElement>) => {
    const f = e.target.files?.[0]
    if (!f) return
    const entry: FileEntry = { file: f, name: f.name }
    if (field === "jd") setJd(entry)
    else setResume(entry)
  }

  const clear = (field: "jd" | "resume") => () => {
    if (field === "jd") { setJd({ file: null, name: "" }); if (jdRef.current) jdRef.current.value = "" }
    else { setResume({ file: null, name: "" }); if (resumeRef.current) resumeRef.current.value = "" }
  }

  const clearAll = () => {
    setJd({ file: null, name: "" })
    setResume({ file: null, name: "" })
    if (jdRef.current) jdRef.current.value = ""
    if (resumeRef.current) resumeRef.current.value = ""
  }

  const handleAnalyze = async () => {
    setError(null)
    setLoading(true)

    const form = new FormData()
    form.append("jd", jd.file!)
    form.append("resume", resume.file!)

    const ac = new AbortController()
    const timeout = setTimeout(() => ac.abort(), 120_000)

    try {
      const apiUrl = import.meta.env.VITE_API_URL || ""
      const res = await fetch(`${apiUrl}/api/analyze`, {
        method: "POST",
        body: form,
        signal: ac.signal,
      })

      if (!res.ok) {
        const body = await res.text().catch(() => "")
        throw new Error(body || `Request failed (${res.status})`)
      }

      clearTimeout(timeout)
      const data: AnalysisResult = await res.json()
      setResult(data)
    } catch (err) {
      clearTimeout(timeout)
      if (err instanceof DOMException && err.name === "AbortError") {
        setError("Request timed out after 120 seconds. The AI service may be slow.")
      } else {
        setError(err instanceof Error ? err.message : "An unexpected error occurred")
      }
    } finally {
      setLoading(false)
    }
  }

  const handleReset = () => {
    setResult(null)
    setError(null)
    setLoading(false)
    clearAll()
  }

  const handleRetry = () => {
    setError(null)
    handleAnalyze()
  }

  const valid = jd.file !== null && resume.file !== null
  const jdAccepted = jd.file !== null
  const resumeAccepted = resume.file !== null

  function DropZone({
    label,
    badge,
    accent,
    accepted,
    file,
    onClear,
    inputRef,
    onChange,
  }: {
    label: string
    badge: string
    accent: "blue" | "amber"
    accepted: boolean
    file: File | null
    onClear: () => void
    inputRef: React.RefObject<HTMLInputElement | null>
    onChange: (e: React.ChangeEvent<HTMLInputElement>) => void
  }) {
    const border = accent === "blue"
      ? "border-blue-500/30 hover:border-blue-400/50"
      : "border-amber-500/30 hover:border-amber-400/50"
    const badgeBg = accent === "blue" ? "bg-blue-500/20 text-blue-300" : "bg-amber-500/20 text-amber-300"

    return (
      <div
        onClick={() => inputRef.current?.click()}
        onDragOver={(e) => e.preventDefault()}
        onDrop={(e) => {
          e.preventDefault()
          const f = e.dataTransfer.files[0]
          if (f && f.type === "application/pdf") {
            const ev = { target: { files: e.dataTransfer.files } } as React.ChangeEvent<HTMLInputElement>
            onChange(ev)
          }
        }}
        className={`relative flex flex-col items-center justify-center gap-2 p-4 sm:p-6 rounded-lg border-2 border-dashed transition-colors cursor-pointer min-h-[100px] sm:min-h-[120px] group ${border}`}
      >
        <span className={`absolute top-2 left-2 text-[10px] font-semibold uppercase tracking-wider px-2 py-0.5 rounded ${badgeBg}`}>
          {badge}
        </span>
        <input
          ref={inputRef}
          type="file"
          accept=".pdf,application/pdf"
          className="hidden"
          onChange={onChange}
        />
        {accepted ? (
          <>
            <FileText className="size-8 text-green-400" />
            <span className="text-xs sm:text-sm text-white/80 truncate max-w-[120px] sm:max-w-[200px]">{file?.name}</span>
            <button
              onClick={(e) => { e.stopPropagation(); onClear() }}
              className="absolute top-2 right-2 text-white/40 hover:text-red-400 transition-colors"
            >
              <X className="size-4" />
            </button>
          </>
        ) : (
          <>
            <Upload className="size-8 text-white/40 group-hover:text-white/60 transition-colors" />
            <span className="text-xs sm:text-sm text-white/50">{label}</span>
            <span className="text-[10px] sm:text-xs text-white/30">PDF only</span>
          </>
        )}
      </div>
    )
  }

  return (
    <div className="relative w-full h-dvh overflow-hidden">
      <Orb
        className="absolute inset-0"
        hue={260}
        hoverIntensity={0.3}
        backgroundColor="#0a0a0f"
      />

      <div className="relative z-10 flex flex-col items-center justify-between h-dvh py-4 sm:py-8 px-3 sm:px-6 lg:px-8">
        <header className="pt-4 sm:pt-8 flex items-center gap-3">
          <Sparkles className="size-6 sm:size-8 lg:size-10 text-white/80" />
          <ShinyText
            text="Resume Analyzer"
            speed={3}
            pauseOnHover
            className="text-2xl sm:text-4xl lg:text-5xl font-bold tracking-tight"
          />
        </header>

        <BorderGlow
          className="w-full max-w-lg"
          backgroundColor="rgba(18, 15, 23, 0.7)"
          glowColor="260 80 80"
          colors={["#c084fc", "#a78bfa", "#818cf8"]}
          glowIntensity={0.8}
          animated={!result && !error && !loading}
        >
          {loading ? (
            <div className="flex flex-col p-4 sm:p-6 lg:p-8 animate-pulse">
              <div className="flex flex-col items-center gap-1 shrink-0">
                <div className="size-16 rounded-full bg-white/10" />
                <div className="h-3 w-20 rounded bg-white/10" />
              </div>
              <div className="flex flex-col gap-6 mt-5">
                <div className="space-y-3">
                  <div className="h-4 w-24 rounded bg-green-500/20" />
                  <div className="space-y-2">
                    <div className="h-3 w-16 rounded bg-white/10" />
                    <div className="h-3 w-full rounded bg-white/10" />
                    <div className="h-3 w-16 rounded bg-white/10 mt-3" />
                    <div className="h-3 w-3/4 rounded bg-white/10" />
                    <div className="h-3 w-16 rounded bg-white/10 mt-3" />
                    <div className="h-3 w-5/6 rounded bg-white/10" />
                  </div>
                </div>
                <div className="space-y-3">
                  <div className="h-4 w-20 rounded bg-red-500/20" />
                  <div className="space-y-2">
                    <div className="h-3 w-16 rounded bg-white/10" />
                    <div className="h-3 w-full rounded bg-white/10" />
                    <div className="h-3 w-16 rounded bg-white/10 mt-3" />
                    <div className="h-3 w-2/3 rounded bg-white/10" />
                  </div>
                </div>
              </div>
              <div className="flex gap-3 justify-center mt-5">
                <div className="h-9 w-24 rounded-md bg-white/10" />
                <div className="h-9 w-32 rounded-md bg-white/10" />
              </div>
            </div>
          ) : error ? (
            <div className="flex flex-col items-center gap-4 p-8 text-center">
              <AlertCircle className="size-10 text-yellow-400" />
              <p className="text-sm text-white/80">{error}</p>
              <Button variant="outline" onClick={handleRetry} className="gap-2">
                <RefreshCw className="size-4" />
                Try Again
              </Button>
            </div>
          ) : result ? (
            <ResultContent result={result} jdName={jd.name} resumeName={resume.name} onReset={handleReset} />
          ) : (
            <div className="flex flex-col gap-3 sm:gap-4 p-4 sm:p-6 lg:p-8">
              <DropZone
                label="Drop Job Description PDF"
                badge="JD"
                accent="blue"
                accepted={jdAccepted}
                file={jd.file}
                onClear={clear("jd")}
                inputRef={jdRef}
                onChange={handleFile("jd")}
              />
              <DropZone
                label="Drop Resume PDF"
                badge="Resume"
                accent="amber"
                accepted={resumeAccepted}
                file={resume.file}
                onClear={clear("resume")}
                inputRef={resumeRef}
                onChange={handleFile("resume")}
              />

              <div className="flex gap-2 sm:gap-3 pt-2">
                <Button
                  disabled={!valid}
                  className="flex-1 gap-2"
                  onClick={handleAnalyze}
                >
                  <Sparkles className="size-4" />
                  Analyze
                </Button>
                <Button
                  variant="outline"
                  onClick={clearAll}
                  disabled={!jdAccepted && !resumeAccepted}
                >
                  Clear
                </Button>
              </div>
            </div>
          )}
        </BorderGlow>

        <footer className="pb-2 sm:pb-4 text-center flex flex-col items-center gap-1">
          <a
            href="https://github.com/spicycoder/resume-analyzer"
            target="_blank"
            rel="noopener noreferrer"
            className="text-white/30 hover:text-white/60 transition-colors text-[10px] sm:text-xs flex items-center gap-1"
          >
            <svg viewBox="0 0 24 24" className="size-3 fill-current"><path d="M12 0C5.37 0 0 5.37 0 12c0 5.31 3.435 9.795 8.205 11.385.6.105.825-.255.825-.57 0-.285-.015-1.23-.015-2.235-3.015.555-3.795-.735-4.035-1.41-.135-.345-.72-1.41-1.23-1.695-.42-.225-1.02-.78-.015-.795.945-.015 1.62.87 1.845 1.23 1.08 1.815 2.805 1.305 3.495.99.105-.78.42-1.305.765-1.605-2.67-.3-5.46-1.335-5.46-5.925 0-1.305.465-2.385 1.23-3.225-.12-.3-.54-1.53.12-3.18 0 0 1.005-.315 3.3 1.23.96-.27 1.98-.405 3-.405s2.04.135 3 .405c2.295-1.56 3.3-1.23 3.3-1.23.66 1.65.24 2.88.12 3.18.765.84 1.23 1.905 1.23 3.225 0 4.605-2.805 5.625-5.475 5.925.435.375.81 1.095.81 2.22 0 1.605-.015 2.895-.015 3.3 0 .315.225.69.825.57A12.02 12.02 0 0 0 24 12c0-6.63-5.37-12-12-12z"/></svg>
            Source
          </a>
          <p className="text-white/40 text-[10px] sm:text-xs leading-relaxed max-w-[260px] sm:max-w-md">
            Files analyzed and discarded immediately — including{" "}
            <RotatingText
              texts={["Resumes", "Job Descriptions"]}
              rotationInterval={2500}
              splitBy="words"
              transition={{ type: "spring", damping: 25, stiffness: 300 }}
              initial={{ y: "100%", opacity: 0 }}
              animate={{ y: 0, opacity: 1 }}
              exit={{ y: "-120%", opacity: 0 }}
              mainClassName="inline-flex text-white/60 font-medium"
              elementLevelClassName="text-white/60"
            />.
          </p>
          <p className="text-white/30 text-[10px] sm:text-xs leading-relaxed max-w-[260px] sm:max-w-md">
            Demo uses a free AI tier — 40 requests/min. If you hit the limit, wait and retry.
          </p>
        </footer>
      </div>
    </div>
  )
}

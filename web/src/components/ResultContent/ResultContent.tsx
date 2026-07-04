import { RotateCcw, Download } from "lucide-react"

import CountUp from "@/components/react-bits/CountUp/CountUp"
import { Button } from "@/components/ui/button"

export interface Flag {
  category: string
  description: string
}

export interface AnalysisResult {
  matchPercentage: number
  greenFlags: Flag[]
  redFlags: Flag[]
}

function scoreColor(pct: number): string {
  if (pct >= 70) return "#22c55e"
  if (pct >= 40) return "#eab308"
  return "#ef4444"
}

function groupByCategory(flags: Flag[]): Record<string, string[]> {
  const map: Record<string, string[]> = {}
  for (const f of flags) {
    if (!map[f.category]) map[f.category] = []
    map[f.category].push(f.description)
  }
  return map
}

function toMarkdown(result: AnalysisResult): string {
  const lines: string[] = []
  lines.push("# Resume Analysis Report")
  lines.push("")
  lines.push(`**Match Score:** ${result.matchPercentage}%`)
  lines.push("")

  if (result.greenFlags.length > 0) {
    lines.push("## Green Flags")
    lines.push("")
    const grouped = groupByCategory(result.greenFlags)
    for (const [category, descriptions] of Object.entries(grouped)) {
      lines.push(`### ${category}`)
      for (const desc of descriptions) {
        lines.push(`- ${desc}`)
      }
      lines.push("")
    }
  }

  if (result.redFlags.length > 0) {
    lines.push("## Red Flags")
    lines.push("")
    const grouped = groupByCategory(result.redFlags)
    for (const [category, descriptions] of Object.entries(grouped)) {
      lines.push(`### ${category}`)
      for (const desc of descriptions) {
        lines.push(`- ${desc}`)
      }
      lines.push("")
    }
  }

  return lines.join("\n")
}

function downloadMarkdown(result: AnalysisResult, jdName: string, resumeName: string) {
  const md = toMarkdown(result)
  const blob = new Blob([md], { type: "text/markdown" })
  const url = URL.createObjectURL(blob)
  const a = document.createElement("a")
  a.href = url
  const jdStem = jdName.replace(/\.[^.]+$/, "")
  const resumeStem = resumeName.replace(/\.[^.]+$/, "")
  a.download = `${jdStem}-${resumeStem}.md`
  document.body.appendChild(a)
  a.click()
  document.body.removeChild(a)
  URL.revokeObjectURL(url)
}

export default function ResultContent({
  result,
  jdName,
  resumeName,
  onReset,
}: {
  result: AnalysisResult
  jdName: string
  resumeName: string
  onReset: () => void
}) {
  const color = scoreColor(result.matchPercentage)

  return (
    <div className="flex flex-col p-4 sm:p-6 lg:p-8 max-h-[70dvh]">
      <div className="flex flex-col items-center gap-1 shrink-0" style={{ color }}>
        <CountUp
          to={result.matchPercentage}
          duration={1.5}
          className="text-5xl sm:text-6xl font-bold tabular-nums"
        />
        <span className="text-xs text-white/40 uppercase tracking-widest">
          Match Score
        </span>
      </div>

      <div className="flex flex-col gap-6 mt-5 min-h-0 overflow-y-auto">
        {result.greenFlags.length > 0 ? (
          <div>
            <h2 className="text-green-400 text-base font-semibold mb-2">Green Flags</h2>
            <ul className="space-y-3 list-disc list-inside">
              {result.greenFlags.map((f, i) => (
                <li key={i} className="text-white/70 text-sm leading-relaxed">
                  <span className="font-semibold text-green-400/80">{f.category}</span>: {f.description}
                </li>
              ))}
            </ul>
          </div>
        ) : null}

        {result.redFlags.length > 0 ? (
          <div>
            <h2 className="text-red-400 text-base font-semibold mb-2">Red Flags</h2>
            <ul className="space-y-3 list-disc list-inside">
              {result.redFlags.map((f, i) => (
                <li key={i} className="text-white/70 text-sm leading-relaxed">
                  <span className="font-semibold text-red-400/80">{f.category}</span>: {f.description}
                </li>
              ))}
            </ul>
          </div>
        ) : null}
      </div>

      <div className="flex gap-3 justify-center shrink-0 mt-5">
        <Button variant="outline" className="gap-2" onClick={() => downloadMarkdown(result, jdName, resumeName)}>
          <Download className="size-4" />
          Export
        </Button>
        <Button variant="outline" onClick={onReset} className="gap-2">
          <RotateCcw className="size-4" />
          New Analysis
        </Button>
      </div>
    </div>
  )
}

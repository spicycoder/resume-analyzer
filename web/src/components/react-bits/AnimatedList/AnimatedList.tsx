import { useRef, useState, useCallback, type ReactNode } from "react"
import { motion } from "motion/react"

interface AnimatedListProps {
  items?: ReactNode[]
  className?: string
}

export default function AnimatedList({
  items = [],
  className = "",
}: AnimatedListProps) {
  const listRef = useRef<HTMLDivElement>(null)
  const [topGradientOpacity, setTopGradientOpacity] = useState(0)
  const [bottomGradientOpacity, setBottomGradientOpacity] = useState(1)

  const handleScroll = useCallback((e: React.UIEvent<HTMLDivElement>) => {
    const { scrollTop, scrollHeight, clientHeight } = e.target as HTMLDivElement
    setTopGradientOpacity(Math.min(scrollTop / 50, 1))
    const bottomDistance = scrollHeight - (scrollTop + clientHeight)
    setBottomGradientOpacity(
      scrollHeight <= clientHeight ? 0 : Math.min(bottomDistance / 50, 1)
    )
  }, [])

  return (
    <div className={`relative ${className}`}>
      <div
        ref={listRef}
        onScroll={handleScroll}
        className="max-h-[280px] overflow-y-auto scrollbar-thin scrollbar-thumb-[#2F293A] scrollbar-track-transparent"
      >
        {items.map((item, index) => (
          <motion.div
            key={index}
            initial={{ scale: 0.7, opacity: 0 }}
            animate={{ scale: 1, opacity: 1 }}
            transition={{ duration: 0.2, delay: 0.1 }}
          >
            {item}
          </motion.div>
        ))}
      </div>
      <div
        className="sticky top-0 left-0 right-0 h-8 pointer-events-none bg-gradient-to-b from-[#120F17] to-transparent -mt-8"
        style={{ opacity: topGradientOpacity }}
      />
      <div
        className="sticky bottom-0 left-0 right-0 h-8 pointer-events-none bg-gradient-to-t from-[#120F17] to-transparent"
        style={{ opacity: bottomGradientOpacity }}
      />
    </div>
  )
}

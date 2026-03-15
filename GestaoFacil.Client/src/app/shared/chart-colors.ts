export const CHART_COLORS = {
  orange: '#f16518',
  dark: '#3b0f03',
  peach: '#ffe5cc',
  green: '#2ecc71',
  red: '#e74c3c',
  blue: '#3498db',
  purple: '#9b59b6',
  teal: '#1abc9c',
  yellow: '#f1c40f',
  pink: '#e91e8a',
};

const PALETTE = [
  '#f16518', '#3498db', '#2ecc71', '#e74c3c', '#9b59b6',
  '#1abc9c', '#f1c40f', '#e91e8a', '#3b0f03', '#95a5a6',
  '#d35400', '#2980b9', '#27ae60', '#c0392b', '#8e44ad',
];

export function categoryPalette(count: number): string[] {
  const colors: string[] = [];
  for (let i = 0; i < count; i++) {
    colors.push(PALETTE[i % PALETTE.length]);
  }
  return colors;
}

import { Chart, registerables } from 'chart.js';

Chart.register(...registerables);

Chart.defaults.font.family = 'Poppins, sans-serif';
Chart.defaults.color = '#3b0f03';

Chart.defaults.plugins.tooltip = {
  ...Chart.defaults.plugins.tooltip,
  backgroundColor: '#3b0f03',
  titleFont: { family: 'Poppins, sans-serif', size: 13, weight: 'bold' },
  bodyFont: { family: 'Poppins, sans-serif', size: 12, weight: 'normal' },
  cornerRadius: 8,
  padding: 10,
};

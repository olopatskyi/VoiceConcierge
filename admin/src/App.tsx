import { Navigate, Route, Routes } from 'react-router-dom';

import { Layout } from './components/Layout';
import { FaqPage } from './pages/FaqPage';
import { PlaygroundPage } from './pages/PlaygroundPage';
import { UnansweredPage } from './pages/UnansweredPage';
import { VoiceConfigPage } from './pages/VoiceConfigPage';

export default function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route index element={<Navigate to="/playground" replace />} />
        <Route path="/playground" element={<PlaygroundPage />} />
        <Route path="/faq" element={<FaqPage />} />
        <Route path="/unanswered" element={<UnansweredPage />} />
        <Route path="/voices" element={<VoiceConfigPage />} />
      </Route>
    </Routes>
  );
}

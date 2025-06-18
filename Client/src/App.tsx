import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { PrivateRoute } from './components/PrivateRoute';

// Pages
import { PublicEvents } from './pages/PublicEvents';
import { SignUp } from './pages/SignUp';
import { ConfirmCode } from './pages/ConfirmCode';
import { Login } from './pages/Login';
import { MyEvents } from './pages/MyEvents';
import { GuestEvents } from './pages/GuestEvents';
import { EventDetail } from './pages/EventDetail';
import { AdminUsers } from './pages/AdminUsers';
import { CreateEvent } from './pages/CreateEvent';        
import { SubscribedEvents } from './pages/SubscribedEvents';  

function App() {
  return (
    <Router>
      <Layout>
        <Routes>
          {/* Public routes */}
          <Route path="/" element={<PublicEvents />} />
          <Route path="/events/public" element={<PublicEvents />} />
          <Route path="/sign-up" element={<SignUp />} />
          <Route path="/confirm-code" element={<ConfirmCode />} />
          <Route path="/login" element={<Login />} />
          <Route path="/events/:id" element={<EventDetail />} />

          {/* Protected routes */}
          <Route 
            path="/events/my" 
            element={
              <PrivateRoute>
                <MyEvents />
              </PrivateRoute>
            } 
          />
          <Route 
            path="/events/guest" 
            element={
              <PrivateRoute>
                <GuestEvents />
              </PrivateRoute>
            } 
          />
          <Route 
            path="/events/create" 
            element={
              <PrivateRoute>
                <CreateEvent />
              </PrivateRoute>
            } 
          />
          <Route 
            path="/events/subscribed" 
            element={
              <PrivateRoute>
                <SubscribedEvents />
              </PrivateRoute>
            } 
          />

          {/* Admin routes */}
          <Route 
            path="/admin/users" 
            element={
              <PrivateRoute requireAdmin={true}>
                <AdminUsers />
              </PrivateRoute>
            } 
          />

          {/* Catch all - redirect to home */}
          <Route path="*" element={<PublicEvents />} />
        </Routes>
      </Layout>
    </Router>
  );
}

export default App;

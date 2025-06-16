import React, { useState, useEffect } from 'react';
import { Event } from '../types/api';
import { apiService } from '../services/api';
import { EventCard } from '../components/EventCard';
import { Users, Search, Loader } from 'lucide-react';

export const GuestEvents: React.FC = () => {
  const [events, setEvents] = useState<Event[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');

  useEffect(() => {
    loadEvents();
  }, []);

  const loadEvents = async () => {
    try {
      setLoading(true);
      const data = await apiService.getAllGuestedEvents();
      setEvents(data);
    } catch (err: any) {
      setError('Failed to load guest events. Please try again.');
      console.error('Error loading guest events:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleUnsubscribe = async (eventId: number) => {
    try {
      await apiService.unsubscribeEvent(eventId);
      await loadEvents(); // Refresh the list
    } catch (err: any) {
      console.error('Error unsubscribing from event:', err);
    }
  };

  const filteredEvents = events.filter(event =>
    event.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
    event.description.toLowerCase().includes(searchTerm.toLowerCase()) ||
    event.location.toLowerCase().includes(searchTerm.toLowerCase())
  );

  if (loading) {
    return (
      <div className="flex items-center justify-center py-12">
        <Loader className="h-8 w-8 animate-spin text-blue-600" />
      </div>
    );
  }

  return (
    <div className="px-4 sm:px-6 lg:px-8">
      <div className="sm:flex sm:items-center">
        <div className="sm:flex-auto">
          <h1 className="text-2xl font-semibold text-gray-900 flex items-center">
            <Users className="h-6 w-6 mr-2 text-teal-600" />
            Guest Events
          </h1>
          <p className="mt-2 text-sm text-gray-700">
            Events you're attending as a guest
          </p>
        </div>
      </div>

      <div className="mt-6">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
          <input
            type="text"
            placeholder="Search guest events..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-10 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-blue-500 focus:border-blue-500"
          />
        </div>
      </div>

      {error && (
        <div className="mt-6 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-md">
          {error}
        </div>
      )}

      <div className="mt-8">
        {filteredEvents.length === 0 ? (
          <div className="text-center py-12">
            <Users className="mx-auto h-12 w-12 text-gray-400" />
            <h3 className="mt-2 text-sm font-medium text-gray-900">No guest events found</h3>
            <p className="mt-1 text-sm text-gray-500">
              {searchTerm 
                ? 'Try adjusting your search terms.' 
                : 'You haven\'t subscribed to any events yet. Browse public events to join some!'
              }
            </p>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {filteredEvents.map((event) => (
              <EventCard
                key={event.id}
                event={event}
                showActions={true}
                onUnsubscribe={handleUnsubscribe}
              />
            ))}
          </div>
        )}
      </div>
    </div>
  );
};
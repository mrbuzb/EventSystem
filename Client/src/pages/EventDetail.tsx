import React, { useState, useEffect, useMemo } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Calendar, MapPin, Users, Clock, Lock, Globe, ArrowLeft,
  Edit, Trash2, UserPlus, UserMinus, Loader
} from 'lucide-react';
import { useAuth } from '../contexts/AuthContext';
import { apiService } from '../services/api';
import { Event } from '../types/api';

export const EventDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { isAuthenticated, user, loading: authLoading } = useAuth();

  const [event, setEvent] = useState<Event | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [actionLoading, setActionLoading] = useState(false);

  useEffect(() => {
    if (!authLoading && id) {
      loadEvent(parseInt(id));
    }
  }, [id, authLoading]);

  const loadEvent = async (eventId: number) => {
    try {
      setLoading(true);
      const data = await apiService.getEventById(eventId);
      setEvent(data);
    } catch {
      setError('Failed to load event details. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const handleSubscribe = async () => {
    if (!event) return;
    try {
      setActionLoading(true);
      await apiService.subscribeEvent(event.id);
      await loadEvent(event.id);
    } finally {
      setActionLoading(false);
    }
  };

  const handleUnsubscribe = async () => {
    if (!event) return;
    try {
      setActionLoading(true);
      await apiService.unsubscribeEvent(event.id);
      await loadEvent(event.id);
    } finally {
      setActionLoading(false);
    }
  };

  const handleDelete = async () => {
    if (!event) return;
    if (window.confirm('Are you sure you want to delete this event?')) {
      try {
        await apiService.deleteEvent(event.id);
        navigate('/events/my');
      } catch (err) {
        console.error('Error deleting event:', err);
      }
    }
  };

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return {
      date: date.toLocaleDateString('en-US', {
        weekday: 'long',
        year: 'numeric',
        month: 'long',
        day: 'numeric'
      }),
      time: date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
    };
  };


  
const isOwner = useMemo(() => {
  if (!event || !user) return false;

  let createdByUserName: string | undefined;

  if (typeof event.createdBy === 'string') {
    createdByUserName = event.createdBy;
  } else if (typeof event.createdBy === 'object' && event.createdBy !== null) {
    createdByUserName = event.createdBy.username;
  }

  console.log("🧪 createdByUserName:", createdByUserName);
  console.log("🧪 user.userName:", user.username);

  return createdByUserName?.toLowerCase() === user.username?.toLowerCase();
}, [event, user]);



  useEffect(() => {
    if (event) {
      console.log('isAuthenticated:', isAuthenticated);
      console.log('user:', user);
      console.log('event:', event);
      console.log('isOwner:', isOwner);
    }
  }, [event, user, isAuthenticated, isOwner]);

  const { date, time } = event ? formatDate(event.date) : { date: '', time: '' };
  const isPublic = event?.type === 0;

  if (loading || authLoading) {
    return (
      <div className="flex items-center justify-center py-12">
        <Loader className="h-8 w-8 animate-spin text-blue-600" />
      </div>
    );
  }

  if (error || !event) {
    return (
      <div className="text-center py-12 px-4">
        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-md">{error || 'Event not found'}</div>
        <button onClick={() => navigate(-1)} className="mt-4 inline-flex items-center px-4 py-2 border border-gray-300 rounded-md text-sm text-gray-700 bg-white hover:bg-gray-50">
          <ArrowLeft className="h-4 w-4 mr-2" /> Go Back
        </button>
      </div>
    );
  }

  return (
    <div className="px-4 sm:px-6 lg:px-8 max-w-4xl mx-auto">
      <div className="mb-6">
        <button onClick={() => navigate(-1)} className="inline-flex items-center px-3 py-2 border border-gray-300 rounded-md text-sm text-gray-700 bg-white hover:bg-gray-50">
          <ArrowLeft className="h-4 w-4 mr-2" /> Back
        </button>
      </div>

      <div className="bg-white shadow-lg rounded-lg overflow-hidden">
        {/* Header */}
        <div className="px-6 py-8 border-b border-gray-200">
          <div className="flex items-start justify-between">
            <div className="flex-1">
              <div className="flex items-center space-x-2 mb-3">
                {isPublic ? <Globe className="h-5 w-5 text-green-600" /> : <Lock className="h-5 w-5 text-orange-600" />}
                <span className={`text-sm font-medium px-2 py-1 rounded-full ${isPublic ? 'bg-green-100 text-green-800' : 'bg-orange-100 text-orange-800'}`}>
                  {isPublic ? 'Public Event' : 'Private Event'}
                </span>
              </div>
              <h1 className="text-3xl font-bold text-gray-900 mb-4">{event.title}</h1>
              <p className="text-gray-600 text-lg leading-relaxed">{event.description}</p>
            </div>

            {isOwner && (
              <div className="flex space-x-2 ml-6">
                <button onClick={() => navigate(`/events/edit/${event.id}`)} className="inline-flex items-center px-3 py-2 border border-gray-300 rounded-md text-sm text-gray-700 bg-white hover:bg-gray-50">
                  <Edit className="h-4 w-4 mr-1" /> Edit
                </button>
                <button onClick={handleDelete} className="inline-flex items-center px-3 py-2 border border-red-300 rounded-md text-sm text-red-700 bg-white hover:bg-red-50">
                  <Trash2 className="h-4 w-4 mr-1" /> Delete
                </button>
              </div>
            )}
          </div>
        </div>

        {/* Event Info */}
        <div className="px-6 py-6">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="space-y-4 text-gray-700">
              <div className="flex items-center"><Calendar className="h-5 w-5 mr-3 text-blue-600" /> {date}</div>
              <div className="flex items-center"><Clock className="h-5 w-5 mr-3 text-blue-600" /> {time}</div>
              <div className="flex items-center"><MapPin className="h-5 w-5 mr-3 text-blue-600" /> {event.location}</div>
              <div className="flex items-center">
  <Users className="h-5 w-5 mr-3 text-blue-600" /> {event.subscribersCount ?? 0} / {event.capasity ?? 0} attendees
  <span className="ml-2 text-sm text-gray-600">
    {(event.capasity ?? 0) - (event.subscribersCount ?? 0)} spot(s) left
  </span>
</div>

            </div>

            {isAuthenticated && !isOwner && (
              <div className="flex flex-col justify-center">
                {event.isSubscribed ? (
                  <button onClick={handleUnsubscribe} disabled={actionLoading} className="inline-flex items-center justify-center px-6 py-3 border border-red-300 rounded-md text-base text-red-700 bg-white hover:bg-red-50 disabled:opacity-50">
                    <UserMinus className="h-5 w-5 mr-2" /> {actionLoading ? 'Unsubscribing...' : 'Unsubscribe'}
                  </button>
                ) : (
                  <button onClick={handleSubscribe} disabled={actionLoading} className="inline-flex items-center justify-center px-6 py-3 border border-transparent rounded-md text-base text-white bg-blue-600 hover:bg-blue-700 disabled:opacity-50">
                    <UserPlus className="h-5 w-5 mr-2" /> {actionLoading ? 'Subscribing...' : 'Subscribe'}
                  </button>
                )}
              </div>
            )}
          </div>
        </div>

        {/* Guests */}
        {event.guestUsers?.length > 0 && (
          <div className="px-6 py-6 border-t border-gray-200">
            <h3 className="text-lg font-medium text-gray-900 mb-4">Invited Guests</h3>
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
              {event.guestUsers.map((guest, index) => (
                <div key={index} className="flex items-center space-x-3 p-3 bg-gray-50 rounded-lg">
                  <div className="h-8 w-8 bg-blue-100 rounded-full flex items-center justify-center">
                    <span className="text-sm font-medium text-blue-600">{guest.firstName[0]?.toUpperCase()}</span>
                  </div>
                  <div>
                    <p className="text-sm font-medium text-gray-900">{guest.firstName}</p>
                    <p className="text-sm text-gray-500 truncate">{guest.email}</p>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

import React from 'react';
import { Link } from 'react-router-dom';
import { Event, EventType } from '../types/api';
import { Calendar, MapPin, Users, Clock, Lock, Globe } from 'lucide-react';

interface EventCardProps {
  event: Event;
  showActions?: boolean;
  onSubscribe?: (eventId: number) => void;
  onUnsubscribe?: (eventId: number) => void;
  onEdit?: (eventId: number) => void;
  onDelete?: (eventId: number) => void;
}

export const EventCard: React.FC<EventCardProps> = ({
  event,
  showActions = false,
  onSubscribe,
  onUnsubscribe,
  onEdit,
  onDelete,
}) => {
  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return {
      date: date.toLocaleDateString(),
      time: date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
    };
  };

  const { date, time } = formatDate(event.date);
  const isPublic = event.type === 0;

  return (
    <div className="bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow duration-200 border border-gray-200 overflow-hidden">
      <div className="p-6">
        <div className="flex items-start justify-between mb-4">
          <div className="flex-1">
            <div className="flex items-center space-x-2 mb-2">
              {isPublic ? (
                <Globe className="h-4 w-4 text-green-600" />
              ) : (
                <Lock className="h-4 w-4 text-orange-600" />
              )}
              <span className={`text-xs font-medium px-2 py-1 rounded-full ${
                isPublic 
                  ? 'bg-green-100 text-green-800' 
                  : 'bg-orange-100 text-orange-800'
              }`}>
                {isPublic ? 'Public' : 'Private'}
              </span>
            </div>
            <Link 
              to={`/events/${event.id}`}
              className="text-xl font-semibold text-gray-900 hover:text-blue-600 transition-colors"
            >
              {event.title}
            </Link>
            <p className="text-gray-600 mt-2 line-clamp-2">{event.description}</p>
          </div>
        </div>

        <div className="space-y-3 mb-4">
          <div className="flex items-center text-gray-600">
            <Calendar className="h-4 w-4 mr-2" />
            <span className="text-sm">{date}</span>
          </div>
          <div className="flex items-center text-gray-600">
            <Clock className="h-4 w-4 mr-2" />
            <span className="text-sm">{time}</span>
          </div>
          <div className="flex items-center text-gray-600">
            <MapPin className="h-4 w-4 mr-2" />
            <span className="text-sm">{event.location}</span>
          </div>
          <div className="flex items-center text-gray-600">
            <Users className="h-4 w-4 mr-2" />
            <p className="text-sm text-gray-600">
  {event.capasity - event.subscribersCount} spot(s) left
</p>

          </div>
        </div>

        {showActions && (
          <div className="flex flex-wrap gap-2 pt-4 border-t border-gray-100">
            {event.isSubscribed ? (
              <button
                onClick={() => onUnsubscribe?.(event.id)}
                className="px-3 py-1 bg-red-100 text-red-700 text-sm font-medium rounded-md hover:bg-red-200 transition-colors"
              >
                Unsubscribe
              </button>
            ) : (
              <button
                onClick={() => onSubscribe?.(event.id)}
                className="px-3 py-1 bg-blue-100 text-blue-700 text-sm font-medium rounded-md hover:bg-blue-200 transition-colors"
              >
                Subscribe
              </button>
            )}
            {onEdit && (
              <button
                onClick={() => onEdit(event.id)}
                className="px-3 py-1 bg-gray-100 text-gray-700 text-sm font-medium rounded-md hover:bg-gray-200 transition-colors"
              >
                Edit
              </button>
            )}
            {onDelete && (
              <button
                onClick={() => onDelete(event.id)}
                className="px-3 py-1 bg-red-100 text-red-700 text-sm font-medium rounded-md hover:bg-red-200 transition-colors"
              >
                Delete
              </button>
            )}
          </div>
        )}
      </div>
    </div>
  );
};
import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { apiService } from '../services/api';
import { EventCreateDto, GuestUserDto } from '../types/api';

export const CreateEvent: React.FC = () => {
  const [title, setTitle] = useState('');
  const [date, setDate] = useState('');
  const [location, setLocation] = useState('');
  const [description, setDescription] = useState('');
  const [capasity, setCapasity] = useState(0);
  const [type, setType] = useState(0); // 0 = Public, 1 = Private

  const [guestUsers, setGuestUsers] = useState<GuestUserDto[]>([
    { email: '', firstName: '' },
  ]);

  const navigate = useNavigate();

  const handleGuestChange = (index: number, field: keyof GuestUserDto, value: string) => {
    const updatedGuests = [...guestUsers];
    updatedGuests[index][field] = value;
    setGuestUsers(updatedGuests);
  };

  const addGuest = () => {
    setGuestUsers([...guestUsers, { email: '', firstName: '' }]);
  };

  const removeGuest = (index: number) => {
    setGuestUsers(guestUsers.filter((_, i) => i !== index));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const newEvent: EventCreateDto = {
      title,
      date: new Date(date),
      location,
      description,
      capasity,
      type,
      guestUsers: guestUsers.filter(g => g.email && g.firstName)
    };

    try {
      await apiService.addEvent(newEvent);
      navigate('/events/my');
    } catch (error) {
      console.error('❌ Failed to create event:', error);
      alert('Failed to create event. Please try again.');
    }
  };

  return (
    <div className="p-4">
      <h2 className="text-xl font-bold mb-4">Create Event</h2>
      <form onSubmit={handleSubmit} className="space-y-4">
        <input
          type="text"
          placeholder="Title"
          value={title}
          onChange={e => setTitle(e.target.value)}
          required
          className="border p-2 w-full"
        />
        <input
          type="datetime-local"
          value={date}
          onChange={e => setDate(e.target.value)}
          required
          className="border p-2 w-full"
        />
        <input
          type="text"
          placeholder="Location"
          value={location}
          onChange={e => setLocation(e.target.value)}
          required
          className="border p-2 w-full"
        />
        <textarea
          placeholder="Description"
          value={description}
          onChange={e => setDescription(e.target.value)}
          required
          className="border p-2 w-full"
        />
        <input
          type="number"
          placeholder="Capasity"
          value={capasity}
          onChange={e => setCapasity(Number(e.target.value))}
          required
          className="border p-2 w-full"
        />
        <select
          value={type}
          onChange={e => setType(Number(e.target.value))}
          className="border p-2 w-full"
        >
          <option value={0}>Public</option>
          <option value={1}>Private</option>
        </select>

        <div className="space-y-2">
          <h3 className="font-semibold">Guest Users</h3>
          {guestUsers.map((guest, index) => (
            <div key={index} className="flex gap-2">
              <input
                type="email"
                placeholder="Email"
                value={guest.email}
                onChange={e => handleGuestChange(index, 'email', e.target.value)}
                className="border p-2 flex-1"
              />
              <input
                type="text"
                placeholder="First Name"
                value={guest.firstName}
                onChange={e => handleGuestChange(index, 'firstName', e.target.value)}
                className="border p-2 flex-1"
              />
              <button type="button" onClick={() => removeGuest(index)} className="text-red-600">✕</button>
            </div>
          ))}
          <button type="button" onClick={addGuest} className="text-blue-600 underline">+ Add Guest</button>
        </div>

        <button type="submit" className="bg-blue-600 text-white px-4 py-2 rounded">
          Create
        </button>
      </form>
    </div>
  );
};

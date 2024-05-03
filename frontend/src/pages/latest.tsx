import { useEffect, useState } from 'react';
import { useUser } from '@auth0/nextjs-auth0/client';
import MessageItem from '../app/components/message-item';
import { FiHeart } from 'react-icons/fi';

interface Message {
  content: string;
  author: string;
}

const LatestMessages: React.FC = () => {
  const { user } = useUser();
  const [messages, setMessages] = useState<Message[]>([]);

  useEffect(() => {
    const fetchMessages = async () => {
      try {
        const response = await fetch('/api/messages');
        const data = await response.json();
        const latestMessages = data.slice(0, 5);
        setMessages(latestMessages);
      } catch (error) {
        console.error('Error fetching messages:', error);
      }
    };

    fetchMessages();
  }, []);

  const handleLike = (index: number) => {
    console.log('Liked message at index:', index);
  };

  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-gray-100">
      <h1 className="text-3xl font-bold mb-6 text-center">Hey {user?.name}</h1>
      <p className="text-small mb-6 text-center">Here are your latest messages</p>
      <div className="w-full max-w-lg">
        {messages.map((message, index) => (
          <div key={index} className="bg-white shadow-md rounded-md p-4 mb-4 flex items-start">
            <div className="flex-grow">
              <MessageItem content={message.content} author={message.author} />
            </div>
            <button
              className="ml-auto text-red-500 hover:text-purple-600 transition-colors focus:outline-none"
              onClick={() => handleLike(index)}
            >
              <FiHeart />
            </button>
          </div>
        ))}
      </div>
      <a href="../" className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-full mr-2">
        Go back
      </a>
    </div>
  );
};

export default LatestMessages;

import React from 'react';
import { FaHeart, FaTrash } from 'react-icons/fa';

interface Message {
    content: string;
    userId: string;
    author?: string;
    likes: number;
    id: number;
    onDelete: (id: number) => void;
}

const MessageItem: React.FC<Message> = ({ content, userId, author, likes, id, onDelete }) => {
    return (
        <div>
            <div className="flex-grow">
                <p className="font-semibold">{author || userId}</p>
                <p>{content}</p>
                <p className="text-xs text-gray-500">{id}</p>
            </div>
            <div className="flex items-center">
                {likes > 0 && (
                    <><span className="text-gray-600 text-sm mr-1">{likes}</span><FaHeart className="text-red-500" /></>
                )}
                <button
                    className="ml-2 text-red-500 hover:text-purple-600 transition-colors focus:outline-none flex items-center"
                    onClick={() => onDelete(id)}
                >
                    <FaTrash className="mr-1" />
                    Delete
                </button>
            </div>
        </div>
    );
};

export default MessageItem;

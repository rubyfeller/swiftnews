import React from 'react';
import { FaHeart, FaTrash } from 'react-icons/fa';

interface Post {
    content: string;
    userId: string;
    author?: string;
    likeCount: number;
    id: number;
}

interface MessageItemProps {
    post: Post;
    onLike: (postId: number) => Promise<void>;
    onDelete: (postId: number) => Promise<void>;
}

const MessageItem: React.FC<MessageItemProps> = ({ post, onLike, onDelete }) => {
    return (
        <div className="bg-white shadow-md rounded-md p-4 mb-4 flex items-start" data-testid={`post-${post.id}`}>
            <div className="flex-grow">
                <p className="font-semibold">{post.author ?? post.userId}</p>
                <p>{post.content}</p>
                <p className="text-xs text-gray-500">{post.id}</p>
            </div>
            <div className="flex items-center">
                {post.likeCount > 0 && (
                    <>
                        <span className="text-gray-600 text-sm mr-1">{post.likeCount}</span>
                        <FaHeart className="text-red-500" />
                    </>
                )}
                <button
                    className="ml-2 text-red-500 hover:text-purple-600 transition-colors focus:outline-none flex items-center"
                    onClick={() => onLike(post.id)}
                    data-testid={`like-button-${post.id}`}
                >
                    <FaHeart className="mr-1" />
                    Like
                </button>
                <button
                    className="ml-2 text-red-500 hover:text-purple-600 transition-colors focus:outline-none flex items-center"
                    onClick={() => onDelete(post.id)}
                    data-testid={`delete-button-${post.id}`}
                >
                    <FaTrash className="mr-1" />
                    Delete
                </button>
            </div>
        </div>
    );
};

export default MessageItem;

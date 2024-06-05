import React, { useState } from 'react';
import { useUser } from "@auth0/nextjs-auth0/client";
import { API_URL_POSTS  } from '../../../config';

interface AddPostFormProps {
  accessToken?: string | null;
  onPostAdded: () => void;
}

const AddPostForm: React.FC<AddPostFormProps> = ({ accessToken, onPostAdded }) => {
  const { user } = useUser();
  const [content, setContent] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      const response = await fetch(`${API_URL_POSTS}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${accessToken}`,
        },
        body: JSON.stringify({ content, userid: user?.sub }),
      });

      if (!response.ok) {
        throw new Error('Failed to add post');
      }

      setContent('');

      onPostAdded();
    } catch (error) {
      console.error('Error adding post:', error);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="mb-6">
      <input
        type="text"
        value={content}
        onChange={(e) => setContent(e.target.value)}
        placeholder="Enter your post content"
        className="border border-gray-300 rounded-md px-4 py-2 mr-2"
        required
      />
      <button
        type="submit"
        className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none"
      >
        Add Post
      </button>
    </form>
  );
};

export default AddPostForm;

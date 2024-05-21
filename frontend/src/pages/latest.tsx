import React, { useEffect, useState, useRef } from 'react';
import MessageItem from '../app/components/message-item';
import { FaHeart } from 'react-icons/fa';
import { useUser } from "@auth0/nextjs-auth0/client";
import AddPostForm from '@/app/components/add-post-form';

interface Post {
  content: string;
  userId: string;
  likeCount: number;
  id: number;
  author?: string;
}

const LatestPosts: React.FC = () => {
  const { user, isLoading, error } = useUser();
  const [posts, setPosts] = useState<Post[]>([]);
  const [accessToken, setAccessToken] = useState<string | null>(null);
  const [postAdded, setPostAdded] = useState<boolean>(false);
  const [page, setPage] = useState<number>(1);
  const [loading, setLoading] = useState<boolean>(false);
  const containerRef = useRef<HTMLDivElement>(null);

  const fetchAccessToken = async () => {
    try {
      const response = await fetch('/api/accessToken');
      const data = await response.json();

      if (!data.accessToken) {
        throw new Error('Access token not found');
      }

      setAccessToken(data.accessToken.accessToken);
    } catch (error) {
      console.error('Error fetching access token:', error);
    }
  };

  const fetchPosts = async (pageNumber: number) => {
    try {
      if (!accessToken) {
        throw new Error('Access token is missing');
      }

      const postsResponse = await fetch(`http://api.localhost:9080/api/posts?page=${pageNumber}&pageSize=10`, {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });

      const postData: Post[] = await postsResponse.json();
      if (postData.length === 0) {
        return;
      }

      const userIds = postData.map(post => post.userId);
      const authors = await fetchAuthorNames(userIds);

      const postsWithAuthors = postData.map(post => ({
        ...post,
        author: authors[post.userId],
      }));

      if (pageNumber === 1) {
        setPosts(postsWithAuthors);
      } else {
        setPosts(prevPosts => [...prevPosts, ...postsWithAuthors]);
      }

      setPage(pageNumber + 1);
      setLoading(false);
    } catch (error) {
      console.error('Error fetching posts:', error);
      setLoading(false);
    }
  };

  const fetchAuthorNames = async (userIds: string[]) => {
    const authors: { [key: string]: string } = {};

    for (const userId of userIds) {
      try {
        if (!userId) continue;

        const userInfoResponse = await fetch(`http://api.localhost:9080/api/user/${userId}`, {
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        });

        if (!userInfoResponse.ok) {
          throw new Error('Failed to fetch user profile');
        }

        const userInfo = await userInfoResponse.json();
        const { username } = userInfo;

        authors[userId] = username;

      } catch (error) {
        console.error(`Error fetching author name for user ID ${userId}:`, error);
      }
    }

    return authors;
  };

  useEffect(() => {
    fetchAccessToken();
  }, []);

  useEffect(() => {
    if (!isLoading && !error && user && accessToken && !postAdded) {
      fetchPosts(page);
    }
  }, [isLoading, error, user, accessToken, postAdded]);

  useEffect(() => {
    const handleScroll = () => {
      if (
        containerRef.current &&
        containerRef.current.scrollTop + containerRef.current.clientHeight >= containerRef.current.scrollHeight &&
        !loading
      ) {
        setLoading(true);
        fetchPosts(page);
      }
    };

    window.addEventListener('scroll', handleScroll);

    return () => {
      window.removeEventListener('scroll', handleScroll);
    };
  }, [loading, page]);

  const handleLike = async (postId: number) => {
    try {
      const response = await fetch(`http://api.localhost:9080/api/l/likes/${postId}`, {
        method: 'POST',
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });

      if (response.ok) {
        fetchPosts(page);
      }
    } catch (error) {
      console.error('Error liking post:', error);
    }
  };

  const handlePostAdded = () => {
    setPostAdded(true);
    setTimeout(() => setPostAdded(false), 1000);
  };

  const handleDelete = async (postId: number) => {
    try {
      const response = await fetch(`http://api.localhost:9080/api/posts/${postId}`, {
        method: 'DELETE',
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });

      if (response.ok) {
        fetchPosts(page);
      }
    } catch (error) {
      console.error('Error deleting post:', error);
    }
  };

  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-gray-100">
      {isLoading && <div>Loading...</div>}
      {!user && <div>Error: not logged in</div>}
      {error && <div>Error: {error.message}</div>}
      {user && !isLoading && !error && (
        <>
          <h1 className="text-3xl font-bold mb-6 mt-8 text-center">Latest posts</h1>
          <div className="w-full max-w-lg" ref={containerRef}>
            {posts.map((post, index) => (
              <div key={index} className="bg-white shadow-md rounded-md p-4 mb-4 flex items-start">
                <div className="flex-grow">
                  <MessageItem content={post.content} userId={post.userId} author={post.author} likes={post.likeCount} id={post.id} onDelete={handleDelete} />
                </div>
                <button
                  className="ml-auto text-red-500 hover:text-purple-600 transition-colors focus:outline-none flex items-center"
                  onClick={() => handleLike(post.id)}
                >
                  <FaHeart className="mr-1" />
                  Like
                </button>
              </div>
            ))}
          </div>
          <AddPostForm accessToken={accessToken} onPostAdded={handlePostAdded} />
        </>
      )}
    </div>
  );
};

export default LatestPosts;

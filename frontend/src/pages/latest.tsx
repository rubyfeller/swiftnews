import React, { useEffect, useState } from 'react';
import MessageItem from '../app/components/message-item';
import { useUser } from "@auth0/nextjs-auth0/client";
import AddPostForm from '@/app/components/add-post-form';
import { API_URL_POSTS, API_URL_LIKE, API_URL_USER } from '../../config';

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
  const [page, setPage] = useState<number>(1);
  const [loading, setLoading] = useState<boolean>(false);
  const [hasMore, setHasMore] = useState<boolean>(true);

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

      const postsResponse = await fetch(`${API_URL_POSTS}?page=${pageNumber}&pageSize=10`, {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });

      const postData: Post[] = await postsResponse.json();
      if (postData.length === 0) {
        setHasMore(false);
        setLoading(false);
        return;
      }

      const userIds = postData.map(post => post.userId);
      const authors = await fetchAuthorNames(userIds);

      const postsWithAuthors = postData.map(post => ({
        ...post,
        author: authors[post.userId],
      }));

      setPosts(prevPosts => pageNumber === 1 ? postsWithAuthors : [...prevPosts, ...postsWithAuthors]);

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

        const userInfoResponse = await fetch(`${API_URL_USER}/${userId}`, {
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
    if (!isLoading && !error && user && accessToken) {
      fetchPosts(1);
    }
  }, [isLoading, error, user, accessToken]);

  useEffect(() => {
    const handleScroll = () => {
      if (
        window.innerHeight + window.scrollY >= document.documentElement.scrollHeight - 100 &&
        !loading &&
        hasMore
      ) {
        setLoading(true);
        fetchPosts(page);
      }
    };

    window.addEventListener('scroll', handleScroll);

    return () => {
      window.removeEventListener('scroll', handleScroll);
    };
  }, [loading, page, hasMore]);

  const handleLike = async (postId: number) => {
    try {
      const response = await fetch(`${API_URL_LIKE}/${postId}`, {
        method: 'POST',
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });

      if (response.ok) {
        fetchPosts(1); // Refresh posts after like
      }
    } catch (error) {
      console.error('Error liking post:', error);
    }
  };

  const handlePostAdded = () => {
    fetchPosts(1); // Refresh posts after a new post is added
  };

  const handleDelete = async (postId: number) => {
    try {
      const response = await fetch(`${API_URL_POSTS}/${postId}`, {
        method: 'DELETE',
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });

      if (response.ok) {
        fetchPosts(1); // Refresh posts after delete
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
          <div className="w-full max-w-lg">
            {posts.map((post) => (
              <MessageItem key={post.id} post={post} onLike={handleLike} onDelete={handleDelete} />
            ))}
          </div>
          <AddPostForm accessToken={accessToken} onPostAdded={handlePostAdded} />
        </>
      )}
    </div>
  );
};

export default LatestPosts;

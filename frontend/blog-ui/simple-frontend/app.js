// API Base URL - Update this to match your backend URL
const API_BASE_URL = 'https://localhost:5055/api';

// Enable CORS for all requests
const fetchWithCors = async (url, options = {}) => {
    const response = await fetch(url, {
        ...options,
        headers: {
            'Content-Type': 'application/json',
            ...options.headers,
        },
        mode: 'cors',
        credentials: 'same-origin'
    });
    return response;
};

// DOM Elements
const blogPostsContainer = document.getElementById('blogPosts');
const loadingElement = document.getElementById('loading');
const searchInput = document.getElementById('searchInput');
const postForm = document.getElementById('postForm');
const postModal = new bootstrap.Modal(document.getElementById('postModal'));
const deleteModal = new bootstrap.Modal(document.getElementById('deleteModal'));
const savePostBtn = document.getElementById('savePostBtn');
const confirmDeleteBtn = document.getElementById('confirmDeleteBtn');

// State
let posts = [];
let postToDeleteId = null;

// Initialize the application
document.addEventListener('DOMContentLoaded', () => {
    loadBlogPosts();
    setupEventListeners();
});

// Set up event listeners
function setupEventListeners() {
    // Search functionality
    searchInput.addEventListener('input', (e) => {
        const searchTerm = e.target.value.toLowerCase();
        filterPosts(searchTerm);
    });

    // Save post form submission
    savePostBtn.addEventListener('click', savePost);
    
    // Delete confirmation
    confirmDeleteBtn.addEventListener('click', deletePost);
}

// Load all blog posts
async function loadBlogPosts() {
    try {
        showLoading(true);
        const response = await fetchWithCors(`${API_BASE_URL}/BlogPosts`);
        if (!response.ok) throw new Error('Failed to fetch blog posts');
        
        posts = await response.json();
        renderBlogPosts(posts);
    } catch (error) {
        console.error('Error loading blog posts:', error);
        showError('Failed to load blog posts. Please try again later.');
    } finally {
        showLoading(false);
    }
}

// Render blog posts to the DOM
function renderBlogPosts(postsToRender) {
    if (!postsToRender || postsToRender.length === 0) {
        blogPostsContainer.innerHTML = `
            <div class="col-12 text-center py-5">
                <h4>No blog posts found</h4>
                <p>Click the "Add New Post" button to create your first post.</p>
            </div>
        `;
        return;
    }

    blogPostsContainer.innerHTML = postsToRender.map(post => `
        <div class="col-md-6 col-lg-4">
            <div class="card blog-card h-100">
                <div class="card-body">
                    <h5 class="card-title">${escapeHtml(post.title)}</h5>
                    <h6 class="card-subtitle mb-2 text-muted">By ${escapeHtml(post.author)}</h6>
                    <p class="card-text">${truncateText(escapeHtml(post.content), 150)}</p>
                </div>
                <div class="card-footer d-flex justify-content-between align-items-center">
                    <small class="text-muted">${formatDate(post.createdAt)}</small>
                    <div>
                        <button class="btn btn-sm btn-outline-primary btn-action me-1" 
                                onclick="editPost(${post.id})">
                            Edit
                        </button>
                        <button class="btn btn-sm btn-outline-danger btn-action" 
                                onclick="confirmDelete(${post.id}, '${escapeHtml(post.title)}')">
                            Delete
                        </button>
                    </div>
                </div>
            </div>
        </div>
    `).join('');
}

// Filter posts based on search term
function filterPosts(searchTerm) {
    if (!searchTerm) {
        renderBlogPosts(posts);
        return;
    }

    const filteredPosts = posts.filter(post => 
        post.title.toLowerCase().includes(searchTerm) || 
        post.author.toLowerCase().includes(searchTerm) ||
        post.content.toLowerCase().includes(searchTerm)
    );
    
    renderBlogPosts(filteredPosts);
}

// Open modal to add a new post
function addNewPost() {
    document.getElementById('postModalLabel').textContent = 'Add New Post';
    document.getElementById('postId').value = '';
    postForm.reset();
    postModal.show();
}

// Edit an existing post
async function editPost(postId) {
    try {
        showLoading(true);
        const response = await fetchWithCors(`${API_BASE_URL}/BlogPosts/${postId}`);
        if (!response.ok) throw new Error('Failed to fetch post');
        
        const post = await response.json();
        
        document.getElementById('postModalLabel').textContent = 'Edit Post';
        document.getElementById('postId').value = post.id;
        document.getElementById('title').value = post.title;
        document.getElementById('author').value = post.author;
        document.getElementById('content').value = post.content;
        
        postModal.show();
    } catch (error) {
        console.error('Error loading post for edit:', error);
        showError('Failed to load post for editing. Please try again.');
    } finally {
        showLoading(false);
    }
}

// Save or update a post
async function savePost() {
    const postId = document.getElementById('postId').value;
    const postData = {
        id: postId ? parseInt(postId) : 0,
        title: document.getElementById('title').value,
        author: document.getElementById('author').value,
        content: document.getElementById('content').value
    };

    try {
        showLoading(true);
        const url = postId ? `${API_BASE_URL}/BlogPosts/${postId}` : `${API_BASE_URL}/BlogPosts`;
        const method = postId ? 'PUT' : 'POST';
        
        const response = await fetchWithCors(url, {
            method: method,
            body: JSON.stringify(postData)
        });

        if (!response.ok) throw new Error('Failed to save post');
        
        postModal.hide();
        await loadBlogPosts();
        showSuccess(`Post ${postId ? 'updated' : 'created'} successfully!`);
    } catch (error) {
        console.error('Error saving post:', error);
        showError('Failed to save post. Please try again.');
    } finally {
        showLoading(false);
    }
}

// Confirm delete post
function confirmDelete(postId, postTitle) {
    postToDeleteId = postId;
    document.querySelector('#deleteModal .modal-body').innerHTML = 
        `Are you sure you want to delete the post "${truncateText(escapeHtml(postTitle), 50)}"? This action cannot be undone.`;
    deleteModal.show();
}

// Delete a post
async function deletePost() {
    if (!postToDeleteId) return;
    
    try {
        showLoading(true);
        const response = await fetchWithCors(`${API_BASE_URL}/BlogPosts/${postToDeleteId}`, {
            method: 'DELETE'
        });

        if (!response.ok) throw new Error('Failed to delete post');
        
        deleteModal.hide();
        await loadBlogPosts();
        showSuccess('Post deleted successfully!');
    } catch (error) {
        console.error('Error deleting post:', error);
        showError('Failed to delete post. Please try again.');
    } finally {
        showLoading(false);
        postToDeleteId = null;
    }
}

// Helper function to format date
function formatDate(dateString) {
    if (!dateString) return '';
    const options = { year: 'numeric', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit' };
    return new Date(dateString).toLocaleDateString('en-US', options);
}

// Helper function to truncate text
function truncateText(text, maxLength) {
    if (!text) return '';
    return text.length > maxLength ? text.substring(0, maxLength) + '...' : text;
}

// Helper function to escape HTML
function escapeHtml(unsafe) {
    if (!unsafe) return '';
    return unsafe
        .toString()
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#039;');
}

// Show/hide loading state
function showLoading(isLoading) {
    loadingElement.style.display = isLoading ? 'block' : 'none';
    if (isLoading) {
        blogPostsContainer.innerHTML = '';
    }
}

// Show success message
function showSuccess(message) {
    const alertDiv = document.createElement('div');
    alertDiv.className = 'alert alert-success alert-dismissible fade show';
    alertDiv.role = 'alert';
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;
    
    const container = document.querySelector('.container');
    container.insertBefore(alertDiv, container.firstChild);
    
    // Auto-dismiss after 5 seconds
    setTimeout(() => {
        alertDiv.classList.remove('show');
        setTimeout(() => alertDiv.remove(), 150);
    }, 5000);
}

// Show error message
function showError(message) {
    const alertDiv = document.createElement('div');
    alertDiv.className = 'alert alert-danger alert-dismissible fade show';
    alertDiv.role = 'alert';
    alertDiv.innerHTML = `
        <strong>Error:</strong> ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;
    
    const container = document.querySelector('.container');
    container.insertBefore(alertDiv, container.firstChild);
    
    // Auto-dismiss after 5 seconds
    setTimeout(() => {
        alertDiv.classList.remove('show');
        setTimeout(() => alertDiv.remove(), 150);
    }, 5000);
}

// Make functions available globally
window.addNewPost = addNewPost;
window.editPost = editPost;
window.confirmDelete = confirmDelete;

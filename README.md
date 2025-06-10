# BookChoice MediaService

BookChoice MediaService is a .NET 8 Web API that provides movie information and search capabilities by integrating with The Movie Database (TMDb) and YouTube. It allows you to retrieve detailed movie data, search for movies, and optionally enrich movie details with related YouTube videos.

---

## Features

- **Get Movie Details:**  
  Retrieve detailed information about a movie using its TMDb or IMDb ID. Optionally, include additional YouTube videos related to the movie title.

- **Search Movies:**  
  Search for movies by title with paginated results.

- **Caching:**  
  Frequently accessed movie and search results are cached in-memory for performance.

- **Error Handling & Logging:**  
  All operations are logged, and errors are handled gracefully with appropriate HTTP status codes.

---

## API Endpoints

### Get Movie Details
GET /api/movies/{id}?includeYouTubeVideos={bool}&maxYouTubeResults={int}


- **id**: TMDb or IMDb movie identifier.
- **includeYouTubeVideos**: (optional, default: false) Whether to include related YouTube videos.
- **maxYouTubeResults**: (optional, default: 10) Maximum number of YouTube videos to include.

---

### Search Movies
GET /api/movies/search?query={query}&page={page}

- **query**: Movie title or keywords.
- **page**: (optional, default: 1) Page number for paginated results.

---

## Configuration

Configuration is managed via `appsettings.json`. You must provide valid TMDb access token and YouTube API key or reuse the existing ones.

Example:

<pre>"TMDbClient": { 
    "BaseUrl": "https://api.themoviedb.org/3/", 
    "AccessToken": "your_tmdb_access_token" 
  }, 
  "YouTubeClient": { 
    "BaseUrl": "https://www.googleapis.com/youtube/v3/", 
    "ApiKey": "your_youtube_api_key" 
  }</pre>

---

## Getting Started

1. **Clone the repository**
2. **Configure the TMDb access token and YouTube API key (or use the existing ones)** in `appsettings.json`
3. **Build and run** the application with or without using Docker
4. **Access the API** at `https://localhost:{port}/api/movies` directly, or view the interactive Swagger documentation at `https://localhost:{port}/swagger/index.html`.

---

## Technologies Used

- .NET 8
- ASP.NET Core Web API
- TMDb API
- YouTube Data API
- Swagger
- Microsoft.Extensions.Caching.Memory for in-memory caching

---

## Testing

Unit tests are provided using xUnit, FluentAssertions, and NSubstitute.  

---

## Suggested Improvements

The following enhancements could further improve the functionality, performance, and maintainability of the BookChoice MediaService:

- **Rate Limiting & Throttling:**  
  Implement rate limiting to protect the API from abuse and to comply with TMDb and YouTube API usage policies.

- **API Authentication & Authorization:**  
  Add authentication (e.g., JWT, API keys) to restrict access to the API endpoints.

- **OpenAPI/Swagger Enhancements:**  
  Add more detailed response examples and error schemas to the Swagger documentation for better API consumer experience.

- **Health Checks & Monitoring:**  
  Integrate health check endpoints and monitoring (e.g., Prometheus, Application Insights) for production readiness.

- **Improved Error Responses:**  
  Return more detailed error information (problem details) using standardized error formats.

- **Localization:**  
  Support multiple languages for movie data and error messages.

- **Configurable and Flexible Caching:**  
  Allow cache expiration settings to be configured, and consider supporting sliding expiration to better control cache lifetimes based on usage patterns.

- **Performance Optimization:**  
  Add distributed caching (e.g., Redis) for scalability in multi-instance deployments.

- **Extensive Unit & Integration Tests:**  
  Increase test coverage, especially for edge cases and integration with external APIs.
  
- **Clearer Result Handling:**  
  Instead of returning null for not found or error cases, return a result object that includes status and error details to make responses less ambiguous.
  
- **Input Validation Improvements:**  
  Use FluentValidation to validate complex input objects, ensuring robust and maintainable input validation logic.
  
- **Exception Handling:**  
  If required catch only specific exception types or use custom exceptions, rather than catching and rethrowing generic exceptions, to improve error clarity and maintainability.

- **Flexible Data Models:**  
  Adapt data models to allow fetching more or less information as needed based on the requirements.

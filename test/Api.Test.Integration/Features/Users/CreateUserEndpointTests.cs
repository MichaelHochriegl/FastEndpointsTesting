using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Api.Domain;
using Api.Features.Users.CreateUser;
using Bogus;
using FastEndpoints;
using FluentAssertions;
using Xunit;

namespace Api.Test.Integration.Features.Users;

/// <summary>
/// We use the <see cref="ApiWebFactory"/> as a <see cref="IClassFixture{TFixture}"/> to get it into the tests.
/// </summary>
public class CreateUserEndpointTests : IClassFixture<ApiWebFactory>
{
    private readonly ApiWebFactory _apiWebFactory;
    private readonly HttpClient _client;

    // With this `Faker` from `Bogus` we will create realistic looking test-data
    private readonly Faker<CreateUserRequest> _userRequestGenerator = new Faker<CreateUserRequest>()
        .RuleFor(x => x.FirstName, faker => faker.Name.FindName())
        .RuleFor(x => x.LastName, faker => faker.Name.LastName())
        .RuleFor(x => x.Email, faker => faker.Internet.Email());

    public CreateUserEndpointTests(ApiWebFactory apiWebFactory)
    {
        _apiWebFactory = apiWebFactory;
        // Creating our `HttpClient` to talk to the API started by the `ApiWebFactory`
        _client = apiWebFactory.CreateClient();
    }

    /// <summary>
    /// Testing the happy path
    /// </summary>
    [Fact]
    public async Task User_with_valid_data_is_created()
    {
        // Arrange
        // Generate a realistic looking user request with `Bogus`
        var user = _userRequestGenerator.Generate();
        
        // Act
        // Executing a `POST` call to the `CreateUserEndpoint`, note that we use the extension method `POSTAsync` for this.
        // `POSTAsync` comes from FastEndpoints and allows to easily call the endpoint by targeting the `Endpoint` class 
        // in one of the generic parameters. It also returns the `HttpResponseMessage`
        // and the actual JSON return type `CreateUserResponse`
         var (response, result) = await _client
             .POSTAsync<Endpoint, CreateUserRequest, CreateUserResponse>(user);

        // Assert
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task User_with_invalid_mail_is_rejected()
    {
        // Arrange
        const string invalidEmail = "invalidEmail";
        var user = _userRequestGenerator.Clone()
            .RuleFor(x => x.Email, invalidEmail)
            .Generate();

        // Act
        var (response, result) = await _client
            .POSTAsync<Endpoint, CreateUserRequest, ErrorResponse>(user);

        // Assert
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Should().NotBeNull();
        result!.Errors.Keys.Should().Contain(nameof(User.Email));
    }
    
    // There are still a lot of cases left to test, they are pretty much the same structure though, so I left them out
    // for brevity
}
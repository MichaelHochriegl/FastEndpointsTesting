using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Api.Features.Users.CreateUser;
using Api.Features.Users.GetUser;
using Bogus;
using FastEndpoints;
using FluentAssertions;
using Xunit;
using Endpoint = Api.Features.Users.GetUser.Endpoint;

namespace Api.Test.Integration.Features.Users;

public class GetUserEndpointTests : IClassFixture<ApiWebFactory>
{
    private readonly ApiWebFactory _apiWebFactory;
    private readonly HttpClient _client;

    private readonly Faker<CreateUserRequest> _userRequestGenerator = new Faker<CreateUserRequest>()
        .CustomInstantiator(faker => new CreateUserRequest(
        faker.Name.FindName(),
        faker.Name.LastName(),
        faker.Internet.Email()));

    public GetUserEndpointTests(ApiWebFactory apiWebFactory)
    {
        _apiWebFactory = apiWebFactory;
        _client = apiWebFactory.CreateClient();
    }

    [Fact]
    public async Task Create_And_Get_User_by_ID_if_present()
    {
        // Arrange
        var user = _userRequestGenerator.Generate();
        var (_, createdUser) = await _client
                .POSTAsync<
                    Api.Features.Users.CreateUser.Endpoint, 
                    CreateUserRequest, 
                    CreateUserResponse>(user);
        createdUser.Should().NotBeNull();
        
        // Act
        var (response, result) = await _client
            .GETAsync<Endpoint, GetUserRequest, GetUserResponse>(new(createdUser!.Id));

        // Assert
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Should().BeEquivalentTo(createdUser);
    }
    
    [Fact]
    public async Task Get_Existing_User_by_ID_if_present()
    {
        // Arrange
        const int seededUserId = 1;
        
        // Act
        var (response, result) = await _client
            .GETAsync<Endpoint, GetUserRequest, GetUserResponse>(new(seededUserId));

        // Assert
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Id.Should().Be(seededUserId);
    }

    [Fact]
    public async Task Get_User_with_unkown_ID_fails()
    {
        // Arrange
        const int unknownId = 999;

        // Act
        var response=
            await _client.GETAsync<Endpoint, GetUserRequest>(new(unknownId));

        // Assert
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
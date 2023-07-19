using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Api.Features.Users.GetUser;
using Bogus;
using FastEndpoints;
using FluentAssertions;
using Xunit;

namespace Api.Test.Integration.Features.Users;

public class GetUserEndpointTests : IClassFixture<ApiWebFactory>
{
    private readonly ApiWebFactory _apiWebFactory;
    private readonly HttpClient _client;

    private readonly Faker<Api.Features.Users.CreateUser.CreateUserRequest> _userRequestGenerator = new Faker<Api.Features.Users.CreateUser.CreateUserRequest>()
        .RuleFor(x => x.FirstName, faker => faker.Name.FindName())
        .RuleFor(x => x.LastName, faker => faker.Name.LastName())
        .RuleFor(x => x.Email, faker => faker.Internet.Email());

    public GetUserEndpointTests(ApiWebFactory apiWebFactory)
    {
        _apiWebFactory = apiWebFactory;
        _client = apiWebFactory.CreateClient();
    }

    [Fact]
    public async Task Get_User_by_ID_if_present()
    {
        // Arrange
        var user = _userRequestGenerator.Generate();
        var (createdResponse, createdUser) = await _client
                .POSTAsync<
                    Api.Features.Users.CreateUser.Endpoint, 
                    Api.Features.Users.CreateUser.CreateUserRequest, 
                    Api.Features.Users.CreateUser.CreateUserResponse>(user);
        createdUser.Should().NotBeNull();
        
        // Act
        var (response, result) = await _client
            .GETAsync<Endpoint, GetUserRequest, GetUserResponse>(new(){ Id = createdUser!.Id});

        // Assert
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Should().BeEquivalentTo(createdUser);
    }

    [Fact]
    public async Task Get_User_with_unkown_ID_fails()
    {
        // Arrange
        const int unknownId = 999;

        // Act
        var response=
            await _client.GETAsync<Endpoint, GetUserRequest>(new() { Id = unknownId });

        // Assert
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
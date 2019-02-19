# FROM instruction, which must be first, initializes a new build stage and sets the Base Image for the remaining instructions.
FROM microsoft/aspnetcore-build:latest AS build-env

# WORKDIR sets the working directory of any remaining RUN, CMD, ENTRYPOINT, COPY and ADD instruction.
WORKDIR /app

# COPY copies the csproj to the container.
COPY *.csproj ./

# RUN executes commands in a new layer on top of the current image and commit the results. In this case, we get the needed dependencies of the project.
RUN dotnet restore

# COPY copies the rest of the files into our container into new layers.
COPY . ./

# RUN executes the command to publish the app into the out directory.
RUN dotnet publish -c Release -o out

# We are using Docker multi-stage build feature, which is why we have another FROM
# FROM instruction for a runtime image
FROM microsoft/aspnetcore:2.2

# WORKDIR sets the working directory of any remaining RUN, CMD, ENTRYPOINT, COPY and ADD instruction.
WORKDIR /app

# COPY copies from the build-env image the content of the /app/out folder to the current image
COPY --from=build-env /app/out .

# ENTRYPOINT instruction allows the container to run as an executable.
ENTRYPOINT ["dotnet", "PriceWatcher.Core.dll"]

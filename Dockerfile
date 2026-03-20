# Stage 1: Build Unity WebGL project
FROM unityci/unity:ubuntu-6000.3.9f1-webgl-3 AS builder

# Serial key credentials (Educational / Plus / Pro license)
ARG UNITY_SERIAL
ARG UNITY_EMAIL
ARG UNITY_PASSWORD

WORKDIR /project

# Copy project files (excludes Library/, Temp/, Builds/ via .dockerignore)
COPY . .

# Activate license, build, then return the seat so it isn't consumed permanently
RUN unity-editor -quit -batchmode -nographics \
      -serial "$UNITY_SERIAL" \
      -username "$UNITY_EMAIL" \
      -password "$UNITY_PASSWORD" \
      -logFile /dev/stdout && \
    unity-editor -quit -batchmode -nographics \
      -projectPath /project \
      -buildTarget WebGL \
      -executeMethod BuildScript.BuildWebGL \
      -logFile /dev/stdout && \
    unity-editor -quit -batchmode -nographics -returnlicense -logFile /dev/stdout

# Stage 2: Serve the WebGL build with nginx
FROM nginx:alpine

COPY --from=builder /project/Builds/WebGL /usr/share/nginx/html

# nginx config: enable gzip for Unity's compressed assets
COPY docker/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]

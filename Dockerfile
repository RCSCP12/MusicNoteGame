# Stage 1: Build Unity WebGL project
FROM unityci/unity:ubuntu-6000.3.9f1-webgl-3 AS builder

ARG UNITY_LICENSE
ARG UNITY_EMAIL
ARG UNITY_PASSWORD

WORKDIR /project

# Copy project files (excludes Library/, Temp/, Builds/ via .dockerignore)
COPY . .

# Activate Unity license and build
RUN echo "$UNITY_LICENSE" > /root/.local/share/unity3d/Unity/Unity_lic.ulf && \
    unity-editor \
      -quit \
      -batchmode \
      -nographics \
      -projectPath /project \
      -buildTarget WebGL \
      -executeMethod BuildScript.BuildWebGL \
      -logFile /dev/stdout || \
    (cat /dev/stdout; exit 1)

# Stage 2: Serve the WebGL build with nginx
FROM nginx:alpine

COPY --from=builder /project/Builds/WebGL /usr/share/nginx/html

# nginx config: enable gzip for Unity's compressed assets
COPY docker/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]

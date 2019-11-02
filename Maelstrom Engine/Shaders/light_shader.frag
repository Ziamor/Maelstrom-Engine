#version 330

out vec4 outputColor;

uniform vec4 lightColor;

void main()
{
    outputColor = lightColor;
}
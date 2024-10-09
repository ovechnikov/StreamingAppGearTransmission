namespace OpenGLWinForms.Shaders;

internal class VertexShader : IShader
{
    public string SourceCode => @"
        #version 330

        layout(location = 0) in vec4 position;
        layout(location = 1) in vec3 normal;

        uniform float uScale;
        uniform float uXpos;
        uniform float uAngle;
        uniform float uRotX;
        uniform float uRotY;

        out vec3    vN;     // normal vector
        out vec3    vL;     // vector from point to light
        out vec3    vE;     // vector from point to eye

        void main(void)
        {
            // Rotation matrices
            mat4 rotationX = mat4(1.0);
            rotationX[1][1] = cos(uRotX);
            rotationX[2][2] = cos(uRotX);
            rotationX[1][2] = -sin(uRotX);
            rotationX[2][1] = sin(uRotX);

            mat4 rotationY = mat4(1.0);
            rotationY[0][0] = cos(uRotY);
            rotationY[2][2] = cos(uRotY);
            rotationY[0][2] = sin(uRotY);
            rotationY[2][0] = -sin(uRotY);

            mat4 rotationZ = mat4(1.0);
            rotationZ[0][0] = cos(uAngle);
            rotationZ[1][1] = cos(uAngle);
            rotationZ[0][1] = -sin(uAngle);
            rotationZ[1][0] = sin(uAngle);

            // Scale matrix
            mat4 scale = mat4(1.0);
            scale[0][0] = uScale;
            scale[1][1] = uScale;
            scale[2][2] = uScale;

            // Translation matrix
            mat4 translation = mat4(1.0);
            translation[3][0] = uXpos;
            translation[3][1] = 0;
            translation[3][2] = 0;

            // Final transformation matrix
            mat4 modelMatrix = rotationX * rotationY * scale * translation * rotationZ;
            mat4 normalMatrix = rotationX * rotationY * scale * translation;

            // Per-fragment lighing
            float   xLight = -6.;
            float   yLight = 10.;
            float   zLight = 10.;

            vec3 LightPosition = vec3(xLight, yLight, zLight);
	        vec4 ECposition = position;
	        vN = normalize((normalMatrix * vec4(normal, 0)).xyz);           // normal vector
	        vL = LightPosition - ECposition.xyz;			// vector from the point
													        // to the light position
	        vE = vec3( 0., 0., 0. ) - ECposition.xyz;		// vector from the point
													        // to the eye position

            gl_Position = modelMatrix * position;
        }
    ";
}

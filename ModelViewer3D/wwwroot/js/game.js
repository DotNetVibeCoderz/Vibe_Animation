
import * as THREE from 'three';
import { GLTFLoader } from 'three/addons/loaders/GLTFLoader.js';

let scene, camera, renderer, mixer, clock;
let player;
let moveForward = false;
let moveBackward = false;
let moveLeft = false;
let moveRight = false;
let animations = {};
let activeAction;
let isLoaded = false;

// Camera Control Variables
let camAngle = 0; // Horizontal angle offset in radians
let camHeight = 5; // Vertical height
let camDist = 12; // Distance from player

// Initialize the 3D world
function initGame() {
    if (isLoaded) return;
    
    console.log("Initializing Game...");

    try {
        clock = new THREE.Clock();

        // Scene
        scene = new THREE.Scene();
        scene.background = new THREE.Color(0x87CEEB); // Sky blue
        scene.fog = new THREE.Fog(0x87CEEB, 20, 100);

        // Camera
        camera = new THREE.PerspectiveCamera(60, window.innerWidth / window.innerHeight, 0.1, 1000);
        camera.position.set(0, 5, 10); 

        // Renderer
        renderer = new THREE.WebGLRenderer({ antialias: true, alpha: false });
        renderer.setSize(window.innerWidth, window.innerHeight);
        renderer.shadowMap.enabled = true;
        renderer.setPixelRatio(window.devicePixelRatio);
        
        // Attach to specific element
        const container = document.getElementById('canvas-container');
        if (container) {
            container.innerHTML = ''; // Clear any existing canvas
            container.appendChild(renderer.domElement);
            console.log("Renderer attached to canvas-container");
        } else {
            document.body.appendChild(renderer.domElement);
        }

        // Lights
        const hemiLight = new THREE.HemisphereLight(0xffffff, 0x444444, 1.0);
        hemiLight.position.set(0, 20, 0);
        scene.add(hemiLight);

        const dirLight = new THREE.DirectionalLight(0xffffff, 1.5);
        dirLight.position.set(10, 20, 10);
        dirLight.castShadow = true;
        scene.add(dirLight);

        // Ground - Grass
        const mesh = new THREE.Mesh( 
            new THREE.PlaneGeometry( 200, 200 ), 
            new THREE.MeshStandardMaterial( { color: 0x4CAF50, depthWrite: true } ) 
        );
        mesh.rotation.x = - Math.PI / 2;
        mesh.receiveShadow = true;
        scene.add(mesh);

        // Village Elements
        createVillage();

        // Event Listeners
        window.addEventListener('resize', onWindowResize);
        document.addEventListener('keydown', (e) => onKey(e, true));
        document.addEventListener('keyup', (e) => onKey(e, false));
        
        // Start Loop
        isLoaded = true;
        animate();
        console.log("Game Initialized Successfully");
        
    } catch (e) {
        console.error("Failed to initialize game:", e);
        alert("Failed to initialize 3D Engine: " + e.message);
    }
}

function createVillage() {
    const houseGeo = new THREE.BoxGeometry(4, 4, 4);
    const roofGeo = new THREE.ConeGeometry(3.5, 2, 4);
    
    for (let i = 0; i < 20; i++) {
        const color = new THREE.Color().setHSL(Math.random(), 0.7, 0.5);
        const houseMat = new THREE.MeshStandardMaterial({ color: color });
        const house = new THREE.Mesh(houseGeo, houseMat);
        
        const x = (Math.random() - 0.5) * 100;
        const z = (Math.random() - 0.5) * 100;

        if (Math.abs(x) < 15 && Math.abs(z) < 15) continue;

        house.position.set(x, 2, z);
        house.castShadow = true;
        house.receiveShadow = true;
        scene.add(house);

        const roofMat = new THREE.MeshStandardMaterial({ color: 0x8B4513 });
        const roof = new THREE.Mesh(roofGeo, roofMat);
        roof.position.set(0, 3, 0);
        roof.rotation.y = Math.PI / 4;
        house.add(roof);
    }
}

function loadModel(url) {
    if (!url) return;
    if (!isLoaded) initGame();

    console.log(`Loading model: ${url}`);
    
    const loader = new GLTFLoader();
    loader.load(url, function (gltf) {
        console.log("Model loaded successfully");
        
        if (player) {
            scene.remove(player);
        }

        player = gltf.scene;
        player.traverse(function (object) {
            if (object.isMesh) object.castShadow = true;
        });

        player.position.set(0, 0, 0);
        scene.add(player);

        // Animations setup
        mixer = new THREE.AnimationMixer(player);
        animations = {};
        
        if (gltf.animations && gltf.animations.length > 0) {
            gltf.animations.forEach((clip) => {
                animations[clip.name] = mixer.clipAction(clip);
            });
            
            const idleAnim = animations['Idle'] || animations['idle'] || animations[Object.keys(animations)[0]];
            if (idleAnim) idleAnim.play();
            activeAction = idleAnim;
        }

    }, undefined, function (error) {
        console.error("Error loading model:", error);
        alert("Failed to load model: " + error.message);
    });
}

// Camera Control Functions (Called from Blazor)
function orbitCamera(direction) {
    const angleSpeed = 0.1; 
    const heightSpeed = 0.5;
    const zoomSpeed = 1.0;

    switch(direction) {
        case 'left': camAngle -= angleSpeed; break;
        case 'right': camAngle += angleSpeed; break;
        case 'up': camHeight += heightSpeed; break;
        case 'down': camHeight -= heightSpeed; break;
        case 'in': camDist -= zoomSpeed; break;
        case 'out': camDist += zoomSpeed; break;
        case 'reset': camAngle = 0; camHeight = 5; camDist = 12; break;
    }
    
    // Clamp limits
    camHeight = Math.max(1, Math.min(20, camHeight));
    camDist = Math.max(5, Math.min(30, camDist));
}

function onKey(event, pressed) {
    switch (event.code) {
        case 'ArrowUp':
        case 'KeyW': moveForward = pressed; break;
        case 'ArrowLeft':
        case 'KeyA': moveLeft = pressed; break;
        case 'ArrowDown':
        case 'KeyS': moveBackward = pressed; break;
        case 'ArrowRight':
        case 'KeyD': moveRight = pressed; break;
    }
}

function onWindowResize() {
    if (!camera || !renderer) return;
    camera.aspect = window.innerWidth / window.innerHeight;
    camera.updateProjectionMatrix();
    renderer.setSize(window.innerWidth, window.innerHeight);
}

function animate() {
    requestAnimationFrame(animate);

    if (!renderer || !scene || !camera) return;

    const dt = clock ? clock.getDelta() : 0.016;

    if (mixer) mixer.update(dt);

    if (player) {
        const speed = 10.0;
        const rotateSpeed = 2.0;
        
        if (moveForward) player.translateZ(speed * dt); 
        if (moveBackward) player.translateZ(-speed * dt);
        if (moveLeft) player.rotateY(rotateSpeed * dt);
        if (moveRight) player.rotateY(-rotateSpeed * dt);

        // Calculate Camera Offset based on Angle/Height/Dist
        // Basic Orbit logic relative to Z-backward
        const xOffset = camDist * Math.sin(camAngle);
        const zOffset = -camDist * Math.cos(camAngle); // Negative because default was behind (-Z)
        
        // Create the relative vector
        const relativeCameraOffset = new THREE.Vector3(xOffset, camHeight, zOffset);
        
        // Apply Player's rotation to this vector so it follows the player
        const cameraOffset = relativeCameraOffset.applyMatrix4(player.matrixWorld);

        camera.position.lerp(cameraOffset, 0.1);
        camera.lookAt(player.position.clone().add(new THREE.Vector3(0, 2, 0))); // Look slightly above feet
    }
    
    renderer.render(scene, camera);
}

window.game = {
    initGame: initGame,
    loadModel: loadModel,
    orbitCamera: orbitCamera, // Expose function
    isReady: () => isLoaded
};

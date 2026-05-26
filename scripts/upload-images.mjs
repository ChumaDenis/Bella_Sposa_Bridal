import { S3Client, PutObjectCommand } from '@aws-sdk/client-s3';
import { readFileSync, readdirSync } from 'fs';
import { join, extname } from 'path';

const ACCOUNT_ID  = '0aa681e66db9f30560cb5db1e498aaa0';
const ACCESS_KEY  = '6736bf364f7b959cda36290b8d629089';
const SECRET_KEY  = 'c15f4f1e9cb8246dcc266b9c7f381903040909c33385a83964da08cbed8519f5';
const BUCKET      = 'bella-sposa';
const PUBLIC_BASE = 'https://pub-72033d04e6fe458286baded587730303.r2.dev';

const IMAGES_DIR = join(new URL('.', import.meta.url).pathname.replace(/^\/([A-Z]:)/, '$1'), '..', 'ui', 'public', 'images');

const CONTENT_TYPES = { '.jpg': 'image/jpeg', '.jpeg': 'image/jpeg', '.png': 'image/png', '.webp': 'image/webp' };

const client = new S3Client({
  region: 'auto',
  endpoint: `https://${ACCOUNT_ID}.r2.cloudflarestorage.com`,
  credentials: { accessKeyId: ACCESS_KEY, secretAccessKey: SECRET_KEY },
  forcePathStyle: true,
});

const files = readdirSync(IMAGES_DIR).filter(f => CONTENT_TYPES[extname(f).toLowerCase()]);

console.log(`Uploading ${files.length} images to R2...\n`);

for (const file of files) {
  const key = `images/${file}`;
  const body = readFileSync(join(IMAGES_DIR, file));
  const contentType = CONTENT_TYPES[extname(file).toLowerCase()] ?? 'image/jpeg';

  try {
    await client.send(new PutObjectCommand({ Bucket: BUCKET, Key: key, Body: body, ContentType: contentType }));
    console.log(`✓  ${file}  →  ${PUBLIC_BASE}/${key}`);
  } catch (e) {
    console.error(`✗  ${file}  FAILED: ${e.message}`);
  }
}

console.log('\nDone. All images are now at:');
console.log(`  ${PUBLIC_BASE}/images/dress-1.jpg  etc.\n`);
console.log('Run this SQL on the production database to update existing photo URLs:');
console.log(`
UPDATE "DressPhotos"
SET "Url" = '${PUBLIC_BASE}' || "Url"
WHERE "Url" LIKE '/images/%';

UPDATE "Collections"
SET "CoverImageUrl" = '${PUBLIC_BASE}' || "CoverImageUrl"
WHERE "CoverImageUrl" LIKE '/images/%';
`);

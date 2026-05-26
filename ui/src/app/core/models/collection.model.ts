export interface CollectionNameDto {
  id: string;
  name: string;
  slug: string;
}

export interface CollectionDto {
  id: string;
  name: string;
  slug: string;
  season: string | null;
  year: number;
  description: string | null;
  coverImageUrl: string | null;
  coverImageUrlMobile: string | null;
  isActive: boolean;
  isDeleted: boolean;
  deletedAt: string | null;
  isFeatured: boolean;
  featuredOrder: number;
  createdAt: string;
  updatedAt: string;
}

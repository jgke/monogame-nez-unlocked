.PHONY: all
all: Nez/patch-applied MonoGame/patch-applied

Nez/patch-applied:
	cd Nez; \
		&& git am --ignore-space-change ../patches/0001-Patched-Nez.patch

MonoGame/patch-applied:
	cd MonoGame; \
		git am --ignore-space-change ../patches/0001-Unlocked-monogame.patch

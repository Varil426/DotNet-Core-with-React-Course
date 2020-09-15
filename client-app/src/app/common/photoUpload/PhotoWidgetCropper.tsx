import React from "react";
import Cropper from "react-cropper";
import "cropperjs/dist/cropper.css";

interface IProps {
	imagePreview: string;
	setCropper: (cropper: Cropper) => void;
}

const PhotoWidgetCropper: React.FC<IProps> = ({ imagePreview, setCropper }) => {
	return (
		<Cropper
			style={{ height: 200, width: "100%" }}
			aspectRatio={1}
			preview=".img-preview"
			src={imagePreview}
			viewMode={1}
			guides={false}
			scalable={true}
			cropBoxMovable={true}
			cropBoxResizable={true}
			onInitialized={(instance) => {
				setCropper(instance);
			}}
		/>
	);
};

export default PhotoWidgetCropper;

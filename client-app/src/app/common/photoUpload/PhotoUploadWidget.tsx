import React, { Fragment, useState, useEffect } from "react";
import { Header, Grid, Button } from "semantic-ui-react";
import { observer } from "mobx-react-lite";
import PhotoWidgetDropZone from "./PhotoWidgetDropzone";
import PhotoWidgetCropper from "./PhotoWidgetCropper";

interface IProps {
	loading: boolean;
	uploadPhoto: (file: Blob) => void;
}

const PhotoUploadWidget: React.FC<IProps> = ({ loading, uploadPhoto }) => {
	const [files, setFiles] = useState<any[]>([]);
	const [image, setImage] = useState<Blob | null>(null);
	const [cropper, setCropper] = useState<Cropper>();

	useEffect(() => {
		return () => {
			files.forEach((file) => URL.revokeObjectURL(file.preview));
		};
	}, [files]);

	useEffect(() => {
		console.log("TEST");
		if (image) uploadPhoto(image);
	}, [image]);

	return (
		<Fragment>
			<Grid>
				<Grid.Column width={4}>
					<Header color="teal" sub content="Step 1 - Add Photo" />
					<PhotoWidgetDropZone setFiles={setFiles} />
				</Grid.Column>
				<Grid.Column width={1} />
				<Grid.Column width={4}>
					<Header sub color="teal" content="Step 2 - Resize image" />
					{files.length > 0 && (
						<PhotoWidgetCropper
							imagePreview={files[0].preview}
							setCropper={setCropper}
						/>
					)}
				</Grid.Column>
				<Grid.Column width={1} />
				<Grid.Column width={4}>
					<Header
						sub
						color="teal"
						content="Step 3 - Preview & Upload"
					/>
					{files.length > 0 && (
						<Fragment>
							<div
								className="img-preview"
								style={{
									minHeight: "200px",
									overflow: "hidden",
								}}
							/>
							<Button.Group widths={2}>
								<Button
									positive
									icon="check"
									loading={loading}
									onClick={() => {
										if (
											cropper &&
											typeof cropper.getCroppedCanvas() !==
												"undefined"
										)
											cropper
												.getCroppedCanvas()
												.toBlob((blob: any) => {
													setImage(blob);
												}, "image/jpeg");
									}}
								/>
								<Button
									icon="close"
									disabled={loading}
									onClick={() => setFiles([])}
								/>
							</Button.Group>
						</Fragment>
					)}
				</Grid.Column>
			</Grid>
		</Fragment>
	);
};

export default observer(PhotoUploadWidget);
